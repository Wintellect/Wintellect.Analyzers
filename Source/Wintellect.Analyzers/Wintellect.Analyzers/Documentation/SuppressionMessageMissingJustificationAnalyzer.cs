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
using System.Linq;

namespace Wintellect.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [DebuggerDisplay("Rule={DiagnosticIds.SuppressionMessageMissingJustificationAnalyzer}")]
    public sealed class SuppressionMessageMissingJustificationAnalyzer : DiagnosticAnalyzer
    {
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIds.SuppressionMessageMissingJustificationAnalyzer,
                                                                             new LocalizableResourceString(nameof(Resources.SuppressionMessageMissingJustificationAnalyzerTitle), Resources.ResourceManager, typeof(Resources)),
                                                                             new LocalizableResourceString(nameof(Resources.SuppressionMessageMissingJustificationAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources)),
                                                                             (new LocalizableResourceString(nameof(Resources.CategoryDocumentation), Resources.ResourceManager, typeof(Resources))).ToString(),
                                                                             DiagnosticSeverity.Warning,
                                                                             true,
                                                                             new LocalizableResourceString(nameof(Resources.SuppressionMessageMissingJustificationAnalyzerDescription), Resources.ResourceManager, typeof(Resources)),
                                                                             "http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect011-SuppressMessageMissingJustification.html");
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            // Request to be called back on all the symbols that can have a SuppressionMessageAttribute applied to them.
            context.RegisterSymbolAction(AnalyzeSuppressMessage,
                                         SymbolKind.Method,
                                         SymbolKind.Field,
                                         SymbolKind.Property,
                                         SymbolKind.NamedType,
                                         SymbolKind.NetModule);
        }

        private void AnalyzeSuppressMessage(SymbolAnalysisContext context)
        {
            // Are we looking at generated code?
            if (!context.Symbol.IsGeneratedOrNonUserCode())
            {
                // Look at the attributes for SuppressMessage.
                var attributes = context.Symbol.GetAttributes();
                for (Int32 i = 0; i < attributes.Count(); i++)
                {
                    if (attributes[i].AttributeClass.Name.Equals("SuppressMessageAttribute"))
                    {
                        Boolean hasJustification = false;

                        // Look for the named parameters for Justification and if it doesn't exist, 
                        // is empty, or has the text <Pending>, report the error.
                        var namedParams = attributes[i].NamedArguments;
                        for (Int32 j = 0; j < namedParams.Count(); j++)
                        {
                            if (namedParams[j].Key.Equals("Justification"))
                            {
                                String textValue = namedParams[j].Value.Value.ToString();
                                if ((String.IsNullOrEmpty(textValue) || (String.Equals(textValue, Resources.PendingText))))
                                {
                                    var diagnostic = Diagnostic.Create(Rule, context.Symbol.Locations[0], context.Symbol.Name);
                                    context.ReportDiagnostic(diagnostic);
                                }
                                hasJustification = true;
                            }
                        }

                        if (!hasJustification)
                        {
                            var diagnostic = Diagnostic.Create(Rule, context.Symbol.Locations[0], context.Symbol.Name);
                            context.ReportDiagnostic(diagnostic);
                        }
                    }
                }
            }
        }
    }
}
