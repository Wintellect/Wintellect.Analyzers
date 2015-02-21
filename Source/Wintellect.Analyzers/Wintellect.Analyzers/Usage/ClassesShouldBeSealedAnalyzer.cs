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
    public sealed class ClassesShouldBeSealedAnalyzer : DiagnosticAnalyzer
    {
       
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIds.ClassesShouldBeSealedAnalyzer, 
                                                                             Resources.ClassesShouldBeSealedAnalyzerTitle, 
                                                                             Resources.ClassesShouldBeSealedAnalyzerMessageFormat, 
                                                                             Resources.CategoryUsage, 
                                                                             DiagnosticSeverity.Info, 
                                                                             true,
                                                                             Resources.ClassesShouldBeSealedAnalyzerDescription,
                                                                             helpLink: "http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect012-ClassesShouldBeSealed.html");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(ClassSealedCheck, SymbolKind.NamedType);
        }

        private void ClassSealedCheck(SymbolAnalysisContext context)
        {
            INamedTypeSymbol symbol = context.Symbol as INamedTypeSymbol;

            // I don't care about generated code.
            if (!symbol.IsGeneratedOrNonUserCode(false))
            {
                // It's all about the class, no structure.
                if ((!symbol.IsValueType) && (!symbol.IsSealed) && (!symbol.IsStatic))
                {
                    var diagnostic = Diagnostic.Create(Rule, symbol.Locations[0], symbol.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}