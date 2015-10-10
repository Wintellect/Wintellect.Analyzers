/*------------------------------------------------------------------------------
Wintellect.Analyzers - .NET Compiler Platform ("Roslyn") Analyzers and CodeFixes
Copyright (c) Wintellect. All rights reserved
Licensed under the Apache License, Version 2.0
See License.txt in the project root for license information
------------------------------------------------------------------------------*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Wintellect.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [DebuggerDisplay("Rule={DiagnosticIds.ClassesShouldBeSealedAnalyzer}")]
    public sealed class ClassesShouldBeSealedAnalyzer : DiagnosticAnalyzer
    {

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIds.ClassesShouldBeSealedAnalyzer,
                                                                             new LocalizableResourceString(nameof(Resources.ClassesShouldBeSealedAnalyzerTitle), Resources.ResourceManager, typeof(Resources)),
                                                                             new LocalizableResourceString(nameof(Resources.ClassesShouldBeSealedAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources)),
                                                                             (new LocalizableResourceString(nameof(Resources.CategoryUsage), Resources.ResourceManager, typeof(Resources))).ToString(),
                                                                             DiagnosticSeverity.Info,
                                                                             true,
                                                                             new LocalizableResourceString(nameof(Resources.ClassesShouldBeSealedAnalyzerDescription), Resources.ResourceManager, typeof(Resources)),
                                                                             "http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect012-ClassesShouldBeSealed.html");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(ClassSealedCheck, SymbolKind.NamedType);
        }

        private void ClassSealedCheck(SymbolAnalysisContext context)
        {
            // I don't care about generated code.
            if (context.IsGeneratedOrNonUserCode())
            {
                return;
            }

            INamedTypeSymbol symbol = context.Symbol as INamedTypeSymbol;

            // It's all about the class, no structure.
            if ((!symbol.IsValueType) && (!symbol.IsSealed) && (!symbol.IsStatic) && (!symbol.IsAbstract))
            {
                var diagnostic = Diagnostic.Create(Rule, symbol.Locations[0], symbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}