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

namespace Wintellect.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AvoidCallingMethodsWithParamArgsAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = DiagnosticIds.AvoidCallingMethodsWithParamArgsAnalyzer;
        // TODO: Needs to be internationalized.
        public const string Title = "Avoid calling methods with param array arguments";
        // TODO: Needs to be internationalized.
        public const string MessageFormat = "Call to a method using a param aray as arguments '{0}'";
        // TODO: Needs to be internationalized.
        public const string Category = "Usage";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId,
                                                                            Title,
                                                                            MessageFormat,
                                                                            Category,
                                                                            DiagnosticSeverity.Info,
                                                                            true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeIdentifier, SyntaxKind.IdentifierName);
        }

        private void AnalyzeIdentifier(SyntaxNodeAnalysisContext context)
        {
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