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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Wintellect.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [DebuggerDisplay("Rule={DiagnosticIds.AvoidPreDefinedTypesAnalyzer}")]
    public sealed class AvoidPreDefinedTypesAnalyzer : DiagnosticAnalyzer
    {
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIds.AvoidPreDefinedTypesAnalyzer,
                                                                             new LocalizableResourceString(nameof(Resources.AvoidPreDefinedTypesAnalyzerTitle), Resources.ResourceManager, typeof(Resources)),
                                                                             new LocalizableResourceString(nameof(Resources.AvoidPreDefinedTypesAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources)),
                                                                             (new LocalizableResourceString(nameof(Resources.CategoryUsage), Resources.ResourceManager, typeof(Resources))).ToString(),
                                                                             DiagnosticSeverity.Warning,
                                                                             true,
                                                                             new LocalizableResourceString(nameof(Resources.AvoidPreDefinedTypesAnalyzerDescription), Resources.ResourceManager, typeof(Resources)),
                                                                             "http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect004-AvoidPredefinedTypes.html");

        private readonly static ImmutableDictionary<String, String> typeMap = new Dictionary<String, String>()
        {
            // This is the fancy, new C# 6.0 initialization.
            ["bool"] = "Boolean",
            ["byte"] = "Byte",
            ["char"] = "Char",
            ["decimal"] = "Decimal",
            ["double"] = "Double",
            ["float"] = "Single",
            ["int"] = "Int32",
            ["long"] = "Int64",
            ["object"] = "Object",
            ["sbyte"] = "SByte",
            ["short"] = "Int16",
            ["string"] = "String",
            ["uint"] = "Uint32",
            ["ulong"] = "Uint64",
            ["ushort"] = "UInt16",
        }.ToImmutableDictionary();

        public static ImmutableDictionary<String, String> TypeMap
        {
            get
            {
                return typeMap;
            }
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            // I only need to look at predefined types.
            context.RegisterSyntaxNodeAction(AnalyzePredefinedType, SyntaxKind.PredefinedType);
        }

        private void AnalyzePredefinedType(SyntaxNodeAnalysisContext context)
        {
            if (context.IsGeneratedOrNonUserCode())
            {
                return;
            }

            PredefinedTypeSyntax predefinedType = context.Node as PredefinedTypeSyntax;

            // Don't touch the void. :)
            if (!predefinedType.ToString().Equals("void", StringComparison.OrdinalIgnoreCase))
            {
                String typeString = predefinedType.ToString();
                String realString = TypeMap[typeString];
                var diagnostic = Diagnostic.Create(Rule,
                                                   predefinedType.GetLocation(),
                                                   typeString,
                                                   realString);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
