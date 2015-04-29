/*------------------------------------------------------------------------------
Wintellect.Analyzers - .NET Compiler Platform ("Roslyn") Analyzers and CodeFixes
Copyright (c) Wintellect. All rights reserved
Licensed under the MIT license
------------------------------------------------------------------------------*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Wintellect.Analyzers
{
    // This rule should support Visual Basic, but I can't get any of the test code
    // working in VS 2015 CTP5 for VB.NET. I'll come back to this on the next CTP.
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    //[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
    [DebuggerDisplay("Rules={DiagnosticIds.AssembliesHaveCompanyAttributeAnalyzer},{DiagnosticIds.AssembliesHaveCopyrightAttributeAnalyzer}...")]
    public sealed class AssemblyAttributeAnalyzer : DiagnosticAnalyzer
    {
        private static DiagnosticDescriptor companyRule = new DiagnosticDescriptor(DiagnosticIds.AssembliesHaveCompanyAttributeAnalyzer,
                                                                                   new LocalizableResourceString(nameof(Resources.AssembliesHaveCompanyAttributeAnalyzerTitle), Resources.ResourceManager, typeof(Resources)),
                                                                                   new LocalizableResourceString(nameof(Resources.AssembliesHaveCompanyAttributeAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources)),
                                                                                   (new LocalizableResourceString(nameof(Resources.CategoryDesign), Resources.ResourceManager, typeof(Resources))).ToString(),
                                                                                   DiagnosticSeverity.Warning,
                                                                                   true,
                                                                                   new LocalizableResourceString(nameof(Resources.AssembliesHaveCompanyAttributeAnalyzerDescription), Resources.ResourceManager, typeof(Resources)),
                                                                                   "http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect006-AssembliesHaveCompanyAttribute.html");

        private static DiagnosticDescriptor copyrightRule = new DiagnosticDescriptor(DiagnosticIds.AssembliesHaveCopyrightAttributeAnalyzer,
                                                                                     new LocalizableResourceString(nameof(Resources.AssembliesHaveCopyrightAttributeAnalyzerTitle), Resources.ResourceManager, typeof(Resources)),
                                                                                     new LocalizableResourceString(nameof(Resources.AssembliesHaveCopyrightAttributeAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources)),
                                                                                     (new LocalizableResourceString(nameof(Resources.CategoryDesign), Resources.ResourceManager, typeof(Resources))).ToString(),
                                                                                     DiagnosticSeverity.Warning,
                                                                                     true,
                                                                                     new LocalizableResourceString(nameof(Resources.AssembliesHaveCopyrightAttributeAnalyzerDescription), Resources.ResourceManager, typeof(Resources)),
                                                                                     "http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect007-AssembliesHaveCopyrightAttribute.html");

        private static DiagnosticDescriptor descriptionRule = new DiagnosticDescriptor(DiagnosticIds.AssembliesHaveDescriptionAttributeAnalyzer,
                                                                                       new LocalizableResourceString(nameof(Resources.AssembliesHaveDescriptionAttributeAnalyzerTitle), Resources.ResourceManager, typeof(Resources)),
                                                                                       new LocalizableResourceString(nameof(Resources.AssembliesHaveDescriptionAttributeAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources)),
                                                                                       (new LocalizableResourceString(nameof(Resources.CategoryDesign), Resources.ResourceManager, typeof(Resources))).ToString(),
                                                                                       DiagnosticSeverity.Warning,
                                                                                       true,
                                                                                       new LocalizableResourceString(nameof(Resources.AssembliesHaveDescriptionAttributeAnalyzerDescription), Resources.ResourceManager, typeof(Resources)),
                                                                                       "http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect008-AssembliesHaveDescriptionAttribute.html");

        private static DiagnosticDescriptor titleRule = new DiagnosticDescriptor(DiagnosticIds.AssembliesHaveTitleAttributeAnalyzer,
                                                                                 new LocalizableResourceString(nameof(Resources.AssembliesHaveTitleAttributeAnalyzerTitle), Resources.ResourceManager, typeof(Resources)),
                                                                                 new LocalizableResourceString(nameof(Resources.AssembliesHaveTitleAttributeAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources)),
                                                                                 (new LocalizableResourceString(nameof(Resources.CategoryDesign), Resources.ResourceManager, typeof(Resources))).ToString(),
                                                                                 DiagnosticSeverity.Warning,
                                                                                 true,
                                                                                 new LocalizableResourceString(nameof(Resources.AssembliesHaveTitleAttributeAnalyzerDescription), Resources.ResourceManager, typeof(Resources)),
                                                                                 "http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect009-AssembliesHaveTitleAttribute.html");
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(companyRule, copyrightRule, descriptionRule, titleRule);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationAction(AnalyzeCompilation);
        }

        private void AnalyzeCompilation(CompilationAnalysisContext context)
        {
            // Get the particular attributes I need to look for.
            var companyAttributeSymbol = KnownTypes.CompanyAttribute(context.Compilation);
            var copyrightAttributeSymbol = KnownTypes.CopyrightAttribute(context.Compilation);
            var descriptionAttributeSymbol = KnownTypes.DescriptionAttribute(context.Compilation);
            var titleAttributeSymbol = KnownTypes.TitleAttribute(context.Compilation);

            // Assume they are all not found.
            Boolean companyAttributeGood = false;
            Boolean copyrightAttributeGood = false;
            Boolean descriptionAttributeGood = false;
            Boolean titleAttributeGood = false;

            // Pound through each attribute in the assembly checking that the specific ones
            // are present and the parameters are not empty.
            foreach (var attribute in context.Compilation.Assembly.GetAttributes())
            {
                if ((companyAttributeSymbol != null) && (attribute.AttributeClass.Equals(companyAttributeSymbol)))
                {
                    companyAttributeGood = CheckAttributeParameter(attribute);
                    continue;
                }

                if ((copyrightAttributeSymbol != null) && (attribute.AttributeClass.Equals(copyrightAttributeSymbol)))
                {
                    copyrightAttributeGood = CheckAttributeParameter(attribute);
                    continue;
                }

                if ((descriptionAttributeSymbol != null) && (attribute.AttributeClass.Equals(descriptionAttributeSymbol)))
                {
                    descriptionAttributeGood = CheckAttributeParameter(attribute);
                    continue;
                }

                if ((titleAttributeSymbol != null) && (attribute.AttributeClass.Equals(titleAttributeSymbol)))
                {
                    titleAttributeGood = CheckAttributeParameter(attribute);
                    continue;
                }
            }

            // If any of the assembly wide attributes are missing or empty, trigger a warning.
            if (!companyAttributeGood)
            {
                context.ReportDiagnostic(Diagnostic.Create(companyRule, Location.None));
            }

            if (!copyrightAttributeGood)
            {
                context.ReportDiagnostic(Diagnostic.Create(copyrightRule, Location.None));
            }

            if (!descriptionAttributeGood)
            {
                context.ReportDiagnostic(Diagnostic.Create(descriptionRule, Location.None));
            }

            if (!titleAttributeGood)
            {
                context.ReportDiagnostic(Diagnostic.Create(titleRule, Location.None));
            }
        }

        private static Boolean CheckAttributeParameter(AttributeData attribute)
        {
            if (attribute.ConstructorArguments.Length == 1)
            {
                String param = attribute.ConstructorArguments[0].Value.ToString();
                if (!String.IsNullOrEmpty(param))
                {
                    return true;
                }
            }

            return false;
        }

        private static class KnownTypes
        {
            public static INamedTypeSymbol CompanyAttribute(Compilation compilation)
            {
                return compilation.GetTypeByMetadataName("System.Reflection.AssemblyCompanyAttribute");
            }

            public static INamedTypeSymbol CopyrightAttribute(Compilation compilation)
            {
                return compilation.GetTypeByMetadataName("System.Reflection.AssemblyCopyrightAttribute");
            }
            public static INamedTypeSymbol DescriptionAttribute(Compilation compilation)
            {
                return compilation.GetTypeByMetadataName("System.Reflection.AssemblyDescriptionAttribute");
            }
            public static INamedTypeSymbol TitleAttribute(Compilation compilation)
            {
                return compilation.GetTypeByMetadataName("System.Reflection.AssemblyTitleAttribute");
            }
        }
    }
}