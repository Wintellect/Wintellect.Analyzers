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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CallAssertMethodsWithMessageParameterCodeFixProvider)), Shared]
    [DebuggerDisplay("CodeFix={DiagnosticIds.CallAssertMethodsWithMessageParameterAnalyzer}")]
    public sealed class CallAssertMethodsWithMessageParameterCodeFixProvider : CodeFixProvider
    {
        private const String actionMessage = "Add test as a message parameter";

        public sealed override ImmutableArray<String> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIds.CallAssertMethodsWithMessageParameterAnalyzer); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            Diagnostic diagnostic = context.Diagnostics.First();
            TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            InvocationExpressionSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().First();

            // Register a code action that will invoke the fix.
            CodeAction codeAction = CodeAction.Create(actionMessage,
                                                      c => CreateMessageFromBooleanAsync(context.Document, declaration, c));
            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private async Task<Document> CreateMessageFromBooleanAsync(Document document, InvocationExpressionSyntax invocationExpr, CancellationToken cancellationToken)
        {
            // Get the single Boolean parameter.
            var argumentList = invocationExpr.ArgumentList as ArgumentListSyntax;
            ExpressionSyntax boolParam = argumentList.Arguments[0].Expression;

            // Instead of building up the nodes by hand, let the great helper
            // SyntaxFactory do the work. The trick to getting the injected code
            // formatted the same is to use the WithAdditionalAnnotations call.
            // That's not obvious, especially since the required 
            // Microsoft.CodeAnalysis.Formatting is not included by the wizard
            // generated code.
            String newCallString = String.Format("Debug.Assert({0}, \"{0}\")",
                                                 boolParam.ToFullString());
            ExpressionSyntax newCall = SyntaxFactory.ParseExpression(newCallString);
            newCall = newCall.WithAdditionalAnnotations(Formatter.Annotation);

            // Poke in our new call and update the document.
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(invocationExpr, newCall);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }
    }
}
