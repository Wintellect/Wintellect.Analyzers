/*------------------------------------------------------------------------------
Wintellect.Analyzers - .NET Compiler Platform ("Roslyn") Analyzers and CodeFixes
Copyright (c) Wintellect. All rights reserved
Licensed under the Apache License, Version 2.0
See License.txt in the project root for license information
------------------------------------------------------------------------------*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Diagnostics;
using System;
using Microsoft.CodeAnalysis.Text;

namespace Wintellect.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [DebuggerDisplay("Rule={DiagnosticIds.CatchBlocksShouldRethrowAnalyzer}")]
    public sealed class CatchBlocksShouldRethrowAnalyzer : DiagnosticAnalyzer
    {

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIds.CatchBlocksShouldRethrowAnalyzer,
                                                                             new LocalizableResourceString(nameof(Resources.CatchBlocksShouldRethrowAnalyzerTitle), Resources.ResourceManager, typeof(Resources)),
                                                                             new LocalizableResourceString(nameof(Resources.CatchBlocksShouldRethrowAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources)),
                                                                             (new LocalizableResourceString(nameof(Resources.CategoryDesign), Resources.ResourceManager, typeof(Resources))).ToString(),
                                                                             DiagnosticSeverity.Info,
                                                                             true,
                                                                             new LocalizableResourceString(nameof(Resources.ClassesShouldBeSealedAnalyzerDescription), Resources.ResourceManager, typeof(Resources)),
                                                                             "http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect014-CatchBlocksShouldRethrow.html");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeCatchBlock, SyntaxKind.CatchClause);
        }

        private void AnalyzeCatchBlock(SyntaxNodeAnalysisContext context)
        {
            // As always skip generated code.
            if (context.IsGeneratedOrNonUserCode())
            {
                return;
            }

            CatchClauseSyntax theCatch = (CatchClauseSyntax)context.Node;

            // I want to be smart about how I look at the catch blocks as the control flow could
            // be slow on very large blocks. Consequently, I only want to look at those blocks
            // that don't have any diagnostics (errors) in them.
            TextSpan span = theCatch.GetLocation().SourceSpan;
            var allDiagnostics = context.SemanticModel.Compilation.GetDiagnostics();
            for (Int32 i = 0; i < allDiagnostics.Length; i++)
            {
                if (allDiagnostics[i].Location.SourceSpan.IntersectsWith(span))
                {
                    return;
                }
            }

            // Take a look at the control flow.
            ControlFlowAnalysis flowAnalysis = context.SemanticModel.AnalyzeControlFlow(theCatch.Block);

            if (flowAnalysis.Succeeded)
            {
                if ((flowAnalysis.ReturnStatements.Length == 0) &&
                    (flowAnalysis.EndPointIsReachable == false) &&
                    (flowAnalysis.EntryPoints.Length == 0))
                {
                    return;
                }

                // Now we know this catch block either eats the exception or has a return.
                var diagnostic = Diagnostic.Create(Rule, theCatch.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}