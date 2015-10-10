/*------------------------------------------------------------------------------
Wintellect.Analyzers - .NET Compiler Platform ("Roslyn") Analyzers and CodeFixes
Copyright (c) Wintellect. All rights reserved
Licensed under the Apache License, Version 2.0
See License.txt in the project root for license information
------------------------------------------------------------------------------*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Wintellect.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [DebuggerDisplay("Rule={DiagnosticIds.ReturningTaskRequiresAsyncAnalyzer}")]
    public sealed class ReturningTaskRequiresAsyncAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The error returned by this rule.
        /// </summary>
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIds.ReturningTaskRequiresAsyncAnalyzer,
                                                                             new LocalizableResourceString(nameof(Resources.ReturningTaskRequiresAsyncAnalyzerTitle), Resources.ResourceManager, typeof(Resources)),
                                                                             new LocalizableResourceString(nameof(Resources.ReturningTaskRequiresAsyncAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources)),
                                                                             (new LocalizableResourceString(nameof(Resources.CategoryUsage), Resources.ResourceManager, typeof(Resources))).ToString(),
                                                                             DiagnosticSeverity.Error,
                                                                             true,
                                                                             new LocalizableResourceString(nameof(Resources.ReturningTaskRequiresAsyncAnalyzerDescription), Resources.ResourceManager, typeof(Resources)),
                                                                             "http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect001-ReturningTaskRequiresAsync.html");


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            // Ask for callbacks only on methods
            context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
        }

        private static void AnalyzeMethod(SymbolAnalysisContext context)
        {
            // I don't care about generated code.
            if (context.IsGeneratedOrNonUserCode())
            {
                return;
            }

            // Look for a return of Task<T> or Task
            // Look at method name
            // If doesn't end in Async, report diagnostic

            IMethodSymbol methodSymbol = (IMethodSymbol)context.Symbol;
            ITypeSymbol returnTypeSymbol = methodSymbol.ReturnType;

            // Make sure we are dealing with the true system type.
            String assemblyName = returnTypeSymbol?.ContainingAssembly?.Identity?.Name;
            if ((assemblyName != null) && (!assemblyName.Contains("mscorlib")))
            {
                return;
            }

            // Account for both Task and Task<T>
            if (!returnTypeSymbol.Name.StartsWith("Task"))
            {
                return;
            }

            // Now look at the method name from the code.
            if (methodSymbol.Name.ToString().EndsWith("Async"))
            {
                return;
            }

            // If here, we found an async method that does not end with "Async"
            var diagnostic = Diagnostic.Create(Rule,
                                               methodSymbol.Locations[0],
                                               methodSymbol.Name);

            context.ReportDiagnostic(diagnostic);
        }
    }
}
