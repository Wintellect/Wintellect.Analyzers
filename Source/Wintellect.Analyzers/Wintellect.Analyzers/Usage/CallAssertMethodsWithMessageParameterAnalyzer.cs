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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Wintellect.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class CallAssertMethodsWithMessageParameterAnalyzer : DiagnosticAnalyzer
    {
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIds.CallAssertMethodsWithMessageParameterAnalyzer,
                                                                             Resources.CallAssertMethodsWithMessageParameterAnalyzerTitle,
                                                                             Resources.CallAssertMethodsWithMessageParameterAnalyzerMessageFormat,
                                                                             Resources.CategoryUsage,
                                                                             DiagnosticSeverity.Error,
                                                                             true,
                                                                             Resources.CallAssertMethodsWithMessageParameterAnalyzerDescription,
                                                                             helpLink: "http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect002-CallAssertMethodsWithMessageParameters.html");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            // Only call this rule on calls to methods. As we are inside the method, we 
            // have to go with the syntax approach.
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
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

            // Let's get serious and double check that we are dealing with 
            // System.Diagnostic.System.Assert. In some scenarios, such as unit 
            // tests the module will be null because the whole tree hasn't been 
            // built. In that case, I'll just have to assume it's the real method.
            IMethodSymbol memberSymbol = context.SemanticModel.GetSymbolInfo(invocationExpr).Symbol as IMethodSymbol;
            if ((memberSymbol != null) && (!(memberSymbol.ContainingModule.ToString().Equals("System.DLL",
                                                                                              StringComparison.OrdinalIgnoreCase))))
            {
                return;
            }

            // How many parameters are there?
            ArgumentListSyntax argumentList = invocationExpr.ArgumentList as ArgumentListSyntax;
            if ((argumentList?.Arguments.Count ?? 0) > 1)
            {
                return;
            }

            // We got us a problem here, boss.
            var diagnostic = Diagnostic.Create(Rule,
                                               invocationExpr.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}