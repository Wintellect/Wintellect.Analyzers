/*------------------------------------------------------------------------------
Wintellect.Analyzers - .NET Compiler Platform ("Roslyn") Analyzers and CodeFixes
Copyright (c) Wintellect. All rights reserved
Licensed under the Apache License, Version 2.0
See License.txt in the project root for license information
------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

namespace Wintellect.Analyzers
{
    [ExportCodeFixProvider("ClassesShouldBeSealedCodeFixProvider",
                            LanguageNames.CSharp)]
    public sealed class ClassesShouldBeSealedCodeFixProvider : CodeFixProvider
    {
        private const String actionMessage = "Make sealed";

        public sealed override ImmutableArray<String> GetFixableDiagnosticIds()
        {
            return ImmutableArray.Create(DiagnosticIds.CallAssertMethodsWithMessageParameterAnalyzer);
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task ComputeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();

            context.RegisterFix(CodeAction.Create(actionMessage,
                                c => AddSealedAsync(context.Document, declaration, c)),
                                diagnostic);
        }

        private async Task<Document> AddSealedAsync(Document document,
                                                    ClassDeclarationSyntax classDeclaration,
                                                    CancellationToken cancellationToken)
        {
            var sealedKeyword = SyntaxFactory.Token(SyntaxKind.SealedKeyword).WithAdditionalAnnotations(Formatter.Annotation);
            var newClass = classDeclaration.AddModifiers(sealedKeyword);

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(classDeclaration, newClass);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }
    }
}