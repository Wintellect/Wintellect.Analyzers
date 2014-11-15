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
    public class CallAssertMethodsWithMessageUnitTests : CodeFixVerifier
    {
        static String debugFalse = @"
using System;
using System.Diagnostics;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.Assert(false);
        }
    }
}";

        static String debugFalseFixed = @"
using System;
using System.Diagnostics;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.Assert(false, ""false"");
        }
    }
}";

        static String debugComplicated = @"
using System;
using System.Diagnostics;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.Assert(DateTime.Now > new DateTime(1));
        }
    }
}
";

        static String debugComplicatedFixed = @"
using System;
using System.Diagnostics;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.Assert(DateTime.Now > new DateTime(1), ""DateTime.Now > new DateTime(1)"");
        }
    }
}
";

        [TestMethod]
        [TestCategory("CallAssertMethodsWithMessageTests")]
        public void TestDebugFalseMethod()
        {
            var expected = new DiagnosticResult
            {
                Id = CallAssertMethodsWithMessageParameterAnalyzer.DiagnosticId,
                Message = CallAssertMethodsWithMessageParameterAnalyzer.MessageFormat,
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 11, 13)
                        }
            };

            VerifyCSharpDiagnostic(debugFalse, expected);
        }

        [TestMethod]
        [TestCategory("CallAssertMethodsWithMessageTests")]
        public void TestDebugComplicatedMethod()
        {
            var expected = new DiagnosticResult
            {
                Id = CallAssertMethodsWithMessageParameterAnalyzer.DiagnosticId,
                Message = CallAssertMethodsWithMessageParameterAnalyzer.MessageFormat,
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 11, 13)
                        }
            };

            VerifyCSharpDiagnostic(debugComplicated, expected);
        }

        [TestMethod]
        [TestCategory("CallAssertMethodsWithMessageTests")]
        public void TestDebugComplicatedFixedMethod()
        {
            VerifyCSharpDiagnostic(debugComplicatedFixed, new DiagnosticResult[0]);
        }

        [TestMethod]
        [TestCategory("CallAssertMethodsWithMessageTests")]
        public void TestDebugFalseFix()
        {
            VerifyCSharpFix(debugFalse, debugFalseFixed);
        }

        [TestMethod]
        [TestCategory("CallAssertMethodsWithMessageTests")]
        public void TestDebugComplicatedFix()
        {
            VerifyCSharpFix(debugComplicated, debugComplicatedFixed);
        }

        [TestMethod]
        [TestCategory("CallAssertMethodsWithMessageTests")]
        public void TestDebugComplicatedNoFix()
        {
            VerifyCSharpFix(debugComplicatedFixed, debugComplicatedFixed);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new CallAssertMethodsWithMessageParameterCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new CallAssertMethodsWithMessageParameterAnalyzer();
        }

    }
}
