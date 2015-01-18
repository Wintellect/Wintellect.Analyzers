/*------------------------------------------------------------------------------
Wintellect.Analyzers - .NET Compiler Platform ("Roslyn") Analyzers and CodeFixes
Copyright (c) Wintellect. All rights reserved
Licensed under the MIT license
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
using System.Threading.Tasks;

namespace Wintellect.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReturningTaskRequiresAsyncAnalyzer : DiagnosticAnalyzer
    {
        // This is the ID shown in the Error window.
        public const String DiagnosticId = DiagnosticIds.ReturningTaskRequiresAsyncAnalyzer;
        // TODO: Needs to be internationalized.
        public const String Title = "All Task or Task<T> returning method names must end with Async";
        // TODO: Needs to be internationalized.
        public const String MessageFormat = "Method name {0} should be renamed {0}Async";
        // TODO: Needs to be internationalized.
        public const String Category = "Naming";

        /// <summary>
        /// The error returned by this rule.
        /// </summary>
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId,
                                                                             Title,
                                                                             MessageFormat,
                                                                             Category,
                                                                             DiagnosticSeverity.Error,
                                                                             isEnabledByDefault: true);


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            // Ask for callbacks only on methods
            context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
        }

        private static void AnalyzeMethod(SymbolAnalysisContext context)
        {
            // Look for a return of Task<T> or Task
            // Look at method name
            // If doesn't end in Async, report diagnostic

            IMethodSymbol methodSymbol = (IMethodSymbol)context.Symbol;
            ITypeSymbol returnTypeSymbol = methodSymbol.ReturnType;

            // Make sure we are dealing with the true system type.
            String assemblyName = returnTypeSymbol.ContainingAssembly.Identity.Name;
            if (!assemblyName.Contains("mscorlib"))
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
