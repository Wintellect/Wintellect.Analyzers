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
    public sealed class UseDebuggerDisplayAttributeAnalyzer : DiagnosticAnalyzer
    {
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIds.UseDebuggerDisplayAttributeAnalyzer, 
                                                                            new LocalizableResourceString(nameof(Resources.UseDebuggerDisplayAttributeTitle), Resources.ResourceManager, typeof(Resources)),
                                                                            new LocalizableResourceString(nameof(Resources.UseDebuggerDisplayAttributeMessageFormat), Resources.ResourceManager, typeof(Resources)),
                                                                            (new LocalizableResourceString(nameof(Resources.CategoryUsage), Resources.ResourceManager, typeof(Resources))).ToString(),
                                                                            DiagnosticSeverity.Warning, 
                                                                            true,
                                                                            new LocalizableResourceString(nameof(Resources.UseDebuggerDisplayAttributeDescription), Resources.ResourceManager, typeof(Resources)),
                                                                            "http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect013-UseDebuggerDisplayAttribute.html");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);
        }

        private void AnalyzeType(SymbolAnalysisContext context)
        {
            INamedTypeSymbol namedSymbol = context.Symbol as INamedTypeSymbol;

            // Right now this analyzer only applies to classes. It could be applicable to structs, but 
            if (namedSymbol.IsValueType)
            {
                return;
            }

            if (namedSymbol.DeclaredAccessibility != Accessibility.Public)
            {
                return;
            }

            // If there's no "state" (fields or properties) there's no sense to have a DebuggerDisplayAttribute.
            var propsOrFields = namedSymbol.GetMembers().Where(n => ((n.Kind == SymbolKind.Property) || (n.Kind == SymbolKind.Field)));
            if (!propsOrFields.Any())
            {
                return;
            }

            // Make sure we look for the parameterless ToString that is on this type.
            IMethodSymbol method = namedSymbol.GetSpecificMethod("ToString", new Type[0]);
            if (method != null)
            {
                return;
            }

            // Grind through the attributes for DebuggerDisplay.
            var attributes = namedSymbol.GetAttributes();

            for (Int32 i = 0; i < attributes.Length; i++)
            {
                String name = attributes[i].AttributeClass.Name;
                if (name.EndsWith("DebuggerDisplayAttribute"))
                {
                    var args = attributes[i].ConstructorArguments;
                    for (Int32 j = 0; j < args.Count(); j++)
                    {
                        String textValue = args[j].Value.ToString();
                        if (!(String.IsNullOrEmpty(textValue)))
                        {
                            return;
                        }
                    }
                }
            }

            var diagnostic = Diagnostic.Create(Rule, namedSymbol.Locations[0], namedSymbol.Name);
            context.ReportDiagnostic(diagnostic);

        }
    }
}