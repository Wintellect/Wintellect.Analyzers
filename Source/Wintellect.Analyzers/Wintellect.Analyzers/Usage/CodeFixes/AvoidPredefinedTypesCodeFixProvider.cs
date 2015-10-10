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
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wintellect.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AvoidPredefinedTypesCodeFixProvider)), Shared]
    [DebuggerDisplay("CodeFix={DiagnosticIds.AvoidPreDefinedTypesAnalyzer}")]
    public sealed class AvoidPredefinedTypesCodeFixProvider : CodeFixProvider
    {
        private const String actionMessage = "Convert to specific type";

        public sealed override ImmutableArray<String> FixableDiagnosticIds => ImmutableArray.Create(DiagnosticIds.AvoidPreDefinedTypesAnalyzer);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            Diagnostic diagnostic = context.Diagnostics.First();
            TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

            PredefinedTypeSyntax errorToken = root.FindToken(diagnosticSpan.Start).Parent as PredefinedTypeSyntax;

            CodeAction codeAction = CodeAction.Create(actionMessage,
                                                      c => ConvertPredefinedToSpecificTypeAsync(context.Document, errorToken, c),
                                                      actionMessage);
            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private async Task<Document> ConvertPredefinedToSpecificTypeAsync(Document document, 
                                                                          PredefinedTypeSyntax errorToken, 
                                                                          CancellationToken c)
        {
            String predefinedType = errorToken.ToString();
            var newType = SyntaxFactory.ParseTypeName(AvoidPreDefinedTypesAnalyzer.TypeMap[predefinedType]);
            newType = newType.WithAdditionalAnnotations(Formatter.Annotation);

            var root = await document.GetSyntaxRootAsync(c);
            var newRoot = root.ReplaceNode(errorToken, newType);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }
    }
}