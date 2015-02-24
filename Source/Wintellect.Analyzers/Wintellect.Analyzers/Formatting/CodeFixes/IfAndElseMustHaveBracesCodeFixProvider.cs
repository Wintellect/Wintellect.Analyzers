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

namespace Wintellect.Analyzers
{
    // This fix was started by Kevin Pilch-Bisson at TechEd Europe 2014.
    // http://channel9.msdn.com/Events/TechEd/Europe/2014/DEV-B345
    // I finished it off and ported it to VS 2015.
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IfAndElseMustHaveBracesCodeFixProvider)), Shared]
    public sealed class IfAndElseMustHaveBracesCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<String> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIds.IfAndElseMustHaveBracesAnalyzer); }
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

            // I'll grab the base class here. As I need to work on two 
            // different SyntaxNode types, I'll do the work in async method to differentiate.
            var token = root.FindToken(diagnosticSpan.Start);
            SyntaxNode errorToken = token.Parent;

            CodeAction codeAction = CodeAction.Create(Resources.IfAndElseMustHaveBracesCodeFixProviderActionMessage,
                                                      c => AddMissingBracesAsync(context.Document, errorToken, c));
            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private async Task<Document> AddMissingBracesAsync(Document document, SyntaxNode errorStatement, CancellationToken c)
        {
            // With two different node types, I'll build up the appropriate new statements for each 
            // type and plug them in with common code.
            SyntaxNode toReplace = null;
            if (errorStatement is IfStatementSyntax)
            {
                IfStatementSyntax newIfStatement = errorStatement as IfStatementSyntax;
                newIfStatement = newIfStatement.WithStatement(SyntaxFactory.Block(newIfStatement.Statement)).WithAdditionalAnnotations(Formatter.Annotation);
                toReplace = newIfStatement;
            }
            else if (errorStatement is ElseClauseSyntax)
            {
                ElseClauseSyntax newElseStatement = errorStatement as ElseClauseSyntax;
                newElseStatement = newElseStatement.WithStatement(SyntaxFactory.Block(newElseStatement.Statement)).WithAdditionalAnnotations(Formatter.Annotation);
                toReplace = newElseStatement;
            }

            if (toReplace != null)
            {
                // Poke in our new call and update the document.
                var root = await document.GetSyntaxRootAsync(c);
                var newRoot = root.ReplaceNode(errorStatement, toReplace);
                var newDocument = document.WithSyntaxRoot(newRoot);
                return newDocument;
            }
            else
            {
                return null;
            }
        }
    }
}