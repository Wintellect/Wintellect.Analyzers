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
using System;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Wintellect.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [DebuggerDisplay("Rule={DiagnosticIds.AvoidCallingMethodsWithParamArgsInLoopsAnalyzer}")]
    public sealed class AvoidCallingMethodsWithParamArgsInLoopsAnalyzer : DiagnosticAnalyzer
    {
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIds.AvoidCallingMethodsWithParamArgsInLoopsAnalyzer,
                                                                             new LocalizableResourceString(nameof(Resources.AvoidCallingMethodsWithParamArgsInLoopsAnalyzerTitle), Resources.ResourceManager, typeof(Resources)),
                                                                             new LocalizableResourceString(nameof(Resources.AvoidCallingMethodsWithParamArgsInLoopsAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources)),
                                                                             (new LocalizableResourceString(nameof(Resources.CategoryPerformance), Resources.ResourceManager, typeof(Resources))).ToString(),
                                                                             DiagnosticSeverity.Info,
                                                                             true,
                                                                             new LocalizableResourceString(nameof(Resources.AvoidCallingMethodsWithParamArgsInLoopsAnalyzerDescription), Resources.ResourceManager, typeof(Resources)),
                                                                             "http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect005-AvoidCallingMethodsWithParamArgInLoops.html");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeIdentifier, SyntaxKind.IdentifierName);
        }

        private void AnalyzeIdentifier(SyntaxNodeAnalysisContext context)
        {
            if (context.IsGeneratedOrNonUserCode())
            {
                return;
            }

            // Look at the method
            // If it has parameters, check if see if the last parameter has the IsParams set.
            IdentifierNameSyntax indentifierName = context.Node as IdentifierNameSyntax;
            IMethodSymbol methodSymbol = context.SemanticModel.GetSymbolInfo(indentifierName).Symbol as IMethodSymbol;
            if (methodSymbol != null)
            {
                Int32 count = methodSymbol.OriginalDefinition.Parameters.Length;
                if (count != 0)
                {
                    if (methodSymbol.OriginalDefinition.Parameters[count - 1].IsParams)
                    {
                        // Only report the error if this call is inside a loop.
                        if (context.Node.IsNodeInALoop())
                        {
                            // We got us a problem here, boss.
                            var diagnostic = Diagnostic.Create(Rule,
                                                               indentifierName.GetLocation(),
                                                               indentifierName.Parent.ToString());
                            context.ReportDiagnostic(diagnostic);
                        }
                    }
                }
            }
        }
    }
}