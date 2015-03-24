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
    [DebuggerDisplay("Rule={DiagnosticIds.CallAssertMethodsWithMessageParameterAnalyzer}")]
    public sealed class CallAssertMethodsWithMessageParameterAnalyzer : DiagnosticAnalyzer
    {
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIds.CallAssertMethodsWithMessageParameterAnalyzer,
                                                                             new LocalizableResourceString(nameof(Resources.CallAssertMethodsWithMessageParameterAnalyzerTitle), Resources.ResourceManager, typeof(Resources)),
                                                                             new LocalizableResourceString(nameof(Resources.CallAssertMethodsWithMessageParameterAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources)),
                                                                             (new LocalizableResourceString(nameof(Resources.CategoryUsage), Resources.ResourceManager, typeof(Resources))).ToString(),
                                                                             DiagnosticSeverity.Error,
                                                                             true,
                                                                             new LocalizableResourceString(nameof(Resources.CallAssertMethodsWithMessageParameterAnalyzerDescription), Resources.ResourceManager, typeof(Resources)),
                                                                             "http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect002-CallAssertMethodsWithMessageParameters.html");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            // Only call this rule on calls to methods. As we are inside the method, we 
            // have to go with the syntax approach.
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.IsGeneratedOrNonUserCode())
            {
                return;
            }

            // Look for calls to Debug.Assert.
            // Ensure it's the
            // Look at the parameters. 
            // If only one, trigger the error.

            // Take a quick peek and see if this is a call to an "Debug.Assert" method.
            InvocationExpressionSyntax invocationExpr = context.Node as InvocationExpressionSyntax;
            if (invocationExpr?.Expression.ToString() != "Debug.Assert")
            {
                return;
            }

            // Let's get serious and double check that we are dealing with System.Diagnostic.System.Assert.
            // In some scenarios, such as unit tests the module will be null because the whole tree hasn't been 
            // built. In that case, I'll just have to assume it's the real method.
            IMethodSymbol memberSymbol = context.SemanticModel.GetSymbolInfo(invocationExpr).Symbol as IMethodSymbol;
            if (memberSymbol != null)
            {
                INamedTypeSymbol classSymbol = memberSymbol.ContainingSymbol as INamedTypeSymbol;
                if (classSymbol != null)
                {
                    if (!classSymbol.IsType(typeof(Debug)))
                    {
                        return;
                    }
                }
            }

            // How many parameters are there?
            ArgumentListSyntax argumentList = invocationExpr.ArgumentList as ArgumentListSyntax;
            if ((argumentList?.Arguments.Count ?? 0) > 1)
            {
                return;
            }

            // We got us a problem here, boss.
            var diagnostic = Diagnostic.Create(Rule, invocationExpr.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}