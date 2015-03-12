/*------------------------------------------------------------------------------
Wintellect.Analyzers - .NET Compiler Platform ("Roslyn") Analyzers and CodeFixes
Copyright (c) Wintellect. All rights reserved
Licensed under the Apache License, Version 2.0
See License.txt in the project root for license information
------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Formatting;
using System.Text;

namespace Wintellect.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseDebuggerDisplayAttributeCodeFixProvider))]
    public sealed class UseDebuggerDisplayAttributeCodeFixProvider : CodeFixProvider
    {
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
            context.RegisterCodeFix(CodeAction.Create("Add DebuggerDisplayAttribute",
                                                        c => AddDebuggerDisplayAttributeCodeFix(context.Document, declaration, c)),
                                    diagnostic);
        }

        private async Task<Document> AddDebuggerDisplayAttributeCodeFix(Document document, 
                                                                        ClassDeclarationSyntax classDecl, 
                                                                        CancellationToken cancellationToken)
        {
            StringBuilder builtDisplayString = new StringBuilder(40);
            builtDisplayString.Append("\"");

            // I really dislike having to do the string search here for a base interface of IEnumerable. However,
            // while you can get the SemanticModel from the document, all the symbol information is empty or null
            // when trying to look up the SimpleBaseTypeSyntax.
            SimpleBaseTypeSyntax baseEnumerable = null;
            if (classDecl.BaseList != null)
            {
                baseEnumerable = classDecl.BaseList.ChildNodes().Where(t => t.ToString().EndsWith("IEnumerable")).First() as SimpleBaseTypeSyntax;
            }

            if (baseEnumerable != null)
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

            var trivia = SyntaxFactory.Comment(@"// TODO: Change the automatically inserted DebuggerDisplay string");

            // There seems to be a bug that if you add the leading trivia as a comment and DO NOT also 
            // include the CR/LF, the trivial is stripped off.
            var list = SyntaxFactory.AttributeList(synList).WithLeadingTrivia(trivia, SyntaxFactory.CarriageReturnLineFeed);

            var classWithNewAttribute = classDecl.AddAttributeLists(list);

            CompilationUnitSyntax root = (CompilationUnitSyntax)await document.GetSyntaxRootAsync(cancellationToken);
            root = root.ReplaceNode(classDecl, classWithNewAttribute);

            root = root.AddUsingIfNotPresent("System.Diagnostics");

            Document newDocument = document.WithSyntaxRoot(root);
            return newDocument;
        }
    }
}