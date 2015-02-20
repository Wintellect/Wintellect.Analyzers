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
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;


namespace Wintellect.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AvoidPreDefinedTypesAnalyzer : DiagnosticAnalyzer
    {
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIds.AvoidPreDefinedTypesAnalyzer,
                                                                             Resources.AvoidPreDefinedTypesAnalyzerTitle,
                                                                             Resources.AvoidPreDefinedTypesAnalyzerMessageFormat,
                                                                             Resources.CategoryUsage,
                                                                             DiagnosticSeverity.Warning,
                                                                             true,
                                                                             Resources.AvoidPreDefinedTypesAnalyzerDescription,
                                                                             helpLink: "http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect004-AvoidPredefinedTypes.html");

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
            // Skip any generated code.
            var symbol = context.SemanticModel.GetEnclosingSymbol(context.Node.SpanStart);
            if (symbol.IsGeneratedOrNonUserCode(false))
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
