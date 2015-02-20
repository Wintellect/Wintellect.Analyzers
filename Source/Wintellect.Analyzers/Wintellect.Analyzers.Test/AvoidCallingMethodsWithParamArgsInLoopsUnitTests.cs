/*------------------------------------------------------------------------------
Wintellect.Analyzers - .NET Compiler Platform ("Roslyn") Analyzers and CodeFixes
Copyright (c) Wintellect. All rights reserved
Licensed under the Apache License, Version 2.0
See License.txt in the project root for license information
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
    public class AvoidCallingMethodsWithParamArgsInLoopsUnitTests : CodeFixVerifier
    {
        const String AvoidCallingMethodsWithParamArgsInLoopsAnalyzerId = "Wintellect005";
        const String AvoidCallingMethodsWithParamArgsInLoopsAnalyzerMessageFormat = "Call to a method using a param array as arguments '{0}' in a loop";

        [TestMethod]
        [TestCategory("AvoidCallingMethodsWithParamArgsInLoopsUnitTests")]
        public void NoLoopTest()
        {
            const String test = @"
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
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("AvoidCallingMethodsWithParamArgsInLoopsUnitTests")]
        public void ForLoopTest()
        {
            const String test = @"
using System;

namespace SomeTests
{
    public class BasicClass
    {
        public String DoSomeParamArrays(Int32 i, String message1, String message2, String message3)
        {
            String returnString = String.Empty;
            for (Int32 j = 0; j < i; j++)
            {
                 returnString += String.Format(""{0}{1}{2}"", message1, message2, message3, message3);
            }
            return returnString;
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = AvoidCallingMethodsWithParamArgsInLoopsAnalyzerId,
                Message = String.Format(AvoidCallingMethodsWithParamArgsInLoopsAnalyzerMessageFormat, "String.Format"),
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 13, 41)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        [TestCategory("AvoidCallingMethodsWithParamArgsInLoopsUnitTests")]
        public void ForEachLoopTest()
        {
            const String test = @"
using System;

namespace SomeTests
{
    public class BasicClass
    {
        public String DoSomeParamArrays(Int32 i, string message1, string message2, string message3)
        {
            String returnString = String.Empty;
            foreach (char c in message1)
            {
                 returnString += String.Format(""{0}{1}{2}"", message1, message2, message3, message3);
            }
            return returnString;
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = AvoidCallingMethodsWithParamArgsInLoopsAnalyzerId,
                Message = String.Format(AvoidCallingMethodsWithParamArgsInLoopsAnalyzerMessageFormat, "String.Format"),
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 13, 41)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        [TestCategory("AvoidCallingMethodsWithParamArgsInLoopsUnitTests")]
        public void WhileLoopTest()
        {
            const String test = @"
using System;

namespace SomeTests
{
    public class BasicClass
    {
        public String DoSomeParamArrays(Int32 i, string message1, string message2, string message3)
        {
            String returnString = String.Empty;
            while (returnString.Length < 100)
            {
                 returnString += String.Format(""{0}{1}{2}"", message1, message2, message3, message3);
            }
            return returnString;
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = AvoidCallingMethodsWithParamArgsInLoopsAnalyzerId,
                Message = String.Format(AvoidCallingMethodsWithParamArgsInLoopsAnalyzerMessageFormat, "String.Format"),
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 13, 41)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        [TestCategory("AvoidCallingMethodsWithParamArgsInLoopsUnitTests")]
        public void DoLoopTest()
        {
            const String test = @"
using System;

namespace SomeTests
{
    public class BasicClass
    {
        public String DoSomeParamArrays(Int32 i, string message1, string message2, string message3)
        {
            String returnString = String.Empty;
            do 
            {
                 returnString += String.Format(""{0}{1}{2}"", message1, message2, message3, message3);
            } while (returnString.Length < 100);
            return returnString;
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = AvoidCallingMethodsWithParamArgsInLoopsAnalyzerId,
                Message = String.Format(AvoidCallingMethodsWithParamArgsInLoopsAnalyzerMessageFormat, "String.Format"),
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 13, 41)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new AvoidCallingMethodsWithParamArgsInLoopsAnalyzer();
        }

    }
}
