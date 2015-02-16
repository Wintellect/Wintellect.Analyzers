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
    [ExportCodeFixProvider("AvoidPredefinedTypesCodeFixProvider", LanguageNames.CSharp)]
    public class AvoidPredefinedTypesCodeFixProvider : CodeFixProvider
    {
        private const String actionMessage = "Convert to specific type";

        public sealed override ImmutableArray<String> GetFixableDiagnosticIds()
        {
            return ImmutableArray.Create(DiagnosticIds.AvoidPreDefinedTypesAnalyzer);
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task ComputeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            Diagnostic diagnostic = context.Diagnostics.First();
            TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

            PredefinedTypeSyntax errorToken = root.FindToken(diagnosticSpan.Start).Parent as PredefinedTypeSyntax;

            CodeAction codeAction = CodeAction.Create(actionMessage,
                                                      c => ConvertPredefinedToSpecificTypeAsync(context.Document, errorToken, c));
            context.RegisterFix(codeAction, diagnostic);
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