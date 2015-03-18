/*------------------------------------------------------------------------------
Wintellect.Analyzers - .NET Compiler Platform ("Roslyn") Analyzers and CodeFixes
Copyright (c) Wintellect. All rights reserved
Licensed under the Apache License, Version 2.0
See License.txt in the project root for license information
------------------------------------------------------------------------------*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System;
using System.Collections;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wintellect.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseDebuggerDisplayAttributeCodeFixProvider))]
    public sealed class UseDebuggerDisplayAttributeCodeFixProvider : CodeFixProvider
    {
        internal static LocalizableResourceString commentString = new LocalizableResourceString(nameof(Resources.UseDebuggerDisplayCommentString), Resources.ResourceManager, typeof(Resources));
        internal static LocalizableResourceString codeFixTitle = new LocalizableResourceString(nameof(Resources.UseDebuggerDisplayCodeFixTitle), Resources.ResourceManager, typeof(Resources));

        public sealed override ImmutableArray<String> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIds.UseDebuggerDisplayAttributeAnalyzer); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Get the class in question.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(CodeAction.Create(codeFixTitle.ToString(),
                                                      c => AddDebuggerDisplayAttributeCodeFix(context.Document, declaration, c)),
                                    diagnostic);
        }

        private async Task<Document> AddDebuggerDisplayAttributeCodeFix(Document document,
                                                                        ClassDeclarationSyntax classDecl,
                                                                        CancellationToken cancellationToken)
        {
            // Grab the symbol for the class so I can see if this is derived from IEnumerable.
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);

            var classSymbol = semanticModel.GetDeclaredSymbol(classDecl) as INamedTypeSymbol;
            Boolean isIEnumerable = classSymbol.IsDerivedFromInterface(typeof(IEnumerable));

            // Start building up the display string.
            StringBuilder builtDisplayString = new StringBuilder(40);
            builtDisplayString.Append("\"");

            if (isIEnumerable)
            {
                // For IEnumerable derived types, show the count of items.
                builtDisplayString.Append("Count={Count()}");
            }
            else
            {
                Int32 addedCount = 0;
                // I'll take a stab at the display string by first looking for two properties and not available, 
                // will default to fields. The analyzer already validated that the class has at least one property
                // or field.
                var properties = classDecl.Members.Where(m => m.IsKind(SyntaxKind.PropertyDeclaration));

                foreach (PropertyDeclarationSyntax prop in properties)
                {
                    if (prop.HasGetter())
                    {
                        if (addedCount > 0)
                        {
                            builtDisplayString.Append(" ");
                        }

                        builtDisplayString.AppendFormat("{0}={{{0}}}", prop.Identifier.ToString());
                        addedCount += 1;
                        if (addedCount == 2)
                        {
                            break;
                        }
                    }
                }

                if (addedCount < 2)
                {
                    var fields = classDecl.Members.Where(m => m.IsKind(SyntaxKind.FieldDeclaration));

                    foreach (FieldDeclarationSyntax field in fields)
                    {
                        if (addedCount > 0)
                        {
                            builtDisplayString.Append(" ");
                        }

                        builtDisplayString.AppendFormat("{0}={{{0}}}", field.FieldName());

                        addedCount += 1;
                        if (addedCount == 2)
                        {
                            break;
                        }
                    }
                }
            }

            builtDisplayString.Append("\"");
            var attr = SyntaxFactoryHelper.Attribute("DebuggerDisplay", builtDisplayString.ToString());
            var synList = SyntaxFactory.SeparatedList(new[] { attr });

            var trivia = SyntaxFactory.Comment(commentString.ToString());

            // Grab any attributes that are already on the class. This will get the trivia as well because I don't
            // want to screw that up.
            var origAttributes = classDecl.AttributeLists;
            
            // There seems to be a bug that if you add the leading trivia as a comment and DO NOT also 
            // include the CR/LF, the trivial is stripped off.
            var list = SyntaxFactory.AttributeList(synList).WithLeadingTrivia(trivia, SyntaxFactory.CarriageReturnLineFeed);

            // Tack the new DebuggerDisplayAttribute on the end.
            var newAttributes = origAttributes.Add(list);

            // Add the new list. The WithAttributeLists call ensures that we don't duplicate attributes that were 
            // already added.
            var classWithNewAttribute = classDecl.WithLeadingTrivia().WithAttributeLists(newAttributes).WithAdditionalAnnotations(Formatter.Annotation);

            // Now to replace the original class node with the new one where I added the DebuggerDisplay attribute.
            CompilationUnitSyntax root = (CompilationUnitSyntax)await document.GetSyntaxRootAsync(cancellationToken);
            root = root.ReplaceNode(classDecl, classWithNewAttribute).WithAdditionalAnnotations(Formatter.Annotation);

            // Ensure the required using is there for the introduction of DebuggerDisplay.
            root = root.AddUsingIfNotPresent("System.Diagnostics");

            Document newDocument = document.WithSyntaxRoot(root);
            return newDocument;
        }
    }
}