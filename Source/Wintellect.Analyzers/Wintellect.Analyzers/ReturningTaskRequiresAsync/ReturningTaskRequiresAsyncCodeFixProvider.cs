/*------------------------------------------------------------------------------
Wintellect.Analyzers - .NET Compiler Platform ("Roslyn") Analyzers and CodeFixes
Copyright (c) Wintellect. All rights reserved
Licensed under the MIT license
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

namespace Wintellect.Analyzers
{
    [ExportCodeFixProvider("Wintellect.ReturningTaskRequiresAsyncCodeFixProvider", 
                            LanguageNames.CSharp), 
     Shared]
    public class ReturningTaskRequiresAsyncCodeFixProvider : CodeFixProvider
    {
        private const String actionMessage = "Rename async method";

        public sealed override ImmutableArray<string> GetFixableDiagnosticIds()
        {
            return ImmutableArray.Create(ReturningTaskRequiresAsyncAnalyzer.DiagnosticId);
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

            // Get the method declaration for the span so we can rename the method.
            MethodDeclarationSyntax methodDeclarationSyntax = root.FindToken(diagnosticSpan.Start).Parent as MethodDeclarationSyntax;

            // Do the rename async.
            CodeAction codeAction = CodeAction.Create(actionMessage, c => RenameMethodAsync(context.Document, methodDeclarationSyntax, c));
            context.RegisterFix(codeAction, diagnostic);
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