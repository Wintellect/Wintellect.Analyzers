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
using Microsoft.CodeAnalysis.Rename;
using System;
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wintellect.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ReturningTaskRequiresAsyncCodeFixProvider)), Shared]
    [DebuggerDisplay("CodeFix={DiagnosticIds.ReturningTaskRequiresAsyncAnalyzer}")]
    public sealed class ReturningTaskRequiresAsyncCodeFixProvider : CodeFixProvider
    {
        private const String actionMessage = "Rename async method";

        public sealed override ImmutableArray<String> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIds.ReturningTaskRequiresAsyncAnalyzer); }
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

            // Get the method declaration for the span so we can rename the method.
            MethodDeclarationSyntax methodDeclarationSyntax = root.FindToken(diagnosticSpan.Start).Parent as MethodDeclarationSyntax;

            // Do the rename async.
            CodeAction codeAction = CodeAction.Create(actionMessage, c => RenameMethodAsync(context.Document, methodDeclarationSyntax, c));
            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private async Task<Solution> RenameMethodAsync(Document document, MethodDeclarationSyntax methodDecl, CancellationToken cancellationToken)
        {
            // Grab the method name and change it.
            var identifierToken = methodDecl.Identifier;
            var newName = identifierToken.Text + "Async";

            // Get the symbol representing the type to be renamed.
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var typeSymbol = semanticModel.GetDeclaredSymbol(methodDecl, cancellationToken);

            // Produce a new solution that has all references to that method renamed, including the declaration.
            var originalSolution = document.Project.Solution;
            var optionSet = originalSolution.Workspace.Options;
            var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, 
                                                             typeSymbol, 
                                                             newName, 
                                                             optionSet, 
                                                             cancellationToken).ConfigureAwait(false);

            // Return the new solution with the renamed method.
            return newSolution;
        }
    }
}