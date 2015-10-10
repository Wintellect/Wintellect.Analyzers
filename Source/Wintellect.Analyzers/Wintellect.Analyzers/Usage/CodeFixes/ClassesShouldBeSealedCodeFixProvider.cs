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
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wintellect.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ClassesShouldBeSealedCodeFixProvider)), Shared]
    [DebuggerDisplay("CodeFix={DiagnosticIds.ClassesShouldBeSealedAnalyzer}")]
    public sealed class ClassesShouldBeSealedCodeFixProvider : CodeFixProvider
    {
        private const String actionMessage = "Make sealed";

        public sealed override ImmutableArray<String> FixableDiagnosticIds => ImmutableArray.Create(DiagnosticIds.ClassesShouldBeSealedAnalyzer);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();

            context.RegisterCodeFix(CodeAction.Create(actionMessage,
                                   c => AddSealedAsync(context.Document, declaration, c),
                                   actionMessage),
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