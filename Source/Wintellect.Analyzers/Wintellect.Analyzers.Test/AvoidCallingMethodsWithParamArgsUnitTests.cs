/*------------------------------------------------------------------------------
Wintellect.Analyzers - .NET Compiler Platform ("Roslyn") Analyzers and CodeFixes
Copyright (c) Wintellect. All rights reserved
Licensed under the MIT license
------------------------------------------------------------------------------*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;

namespace Wintellect.Analyzers.Test
{
    [TestClass]
    public class AvoidCallingMethodsWithParamArgsUnitTests : CodeFixVerifier
    {
        static String callParamArrayMethod = @"
using System;

namespace SomeTests
{
    public class BasicClass
    {
        public String DoSomeParamArrays(string message1, string message2, string message3)
        {
            return String.Format(""{0}{1}{2}"", message1, message2, message3, message3);
        }
    }
}
";
        const String AvoidCallingMethodsWithParamArgsAnalyzerId = "Wintellect005";
        const String AvoidCallingMethodsWithParamArgsAnalyzerMessageFormat = "Call to a method using a param aray as arguments '{0}'";

        [TestMethod]
        [TestCategory("AvoidCallingMethodsWithParamArgsUnitTests")]
        public void TestCallParamArrayMethod()
        {
            var expected = new DiagnosticResult
            {
                Id = AvoidCallingMethodsWithParamArgsAnalyzerId,
                Message = String.Format(AvoidCallingMethodsWithParamArgsAnalyzerMessageFormat, "String.Format"),
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 10, 27)
                        }
            };

            VerifyCSharpDiagnostic(callParamArrayMethod, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new AvoidCallingMethodsWithParamArgsAnalyzer();
        }

    }
}
