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
    public class AvoidPreDefinedTypesUnitTests : CodeFixVerifier
    {
        static String singlePredefinedType = @"
using System;

namespace SomeTests
{
    public class BasicClass
    {
        public void SomeWork(string message)
        {
            Console.WriteLine(message);
        }
    }
}
";

        static String singlePredefinedTypeFixed = @"
using System;

namespace SomeTests
{
    public class BasicClass
    {
        public void SomeWork(String message)
        {
            Console.WriteLine(message);
        }
    }
}
";

        static String multiplePredefinedType = @"
using System;

namespace SomeTests
{
    public class BasicClass
    {
        private object o;
        private int i;

        public void SomeWork(string message)
        {
            i += 1;
            lock(o)
            {
                Console.WriteLine(message);
            }
        }
    }
}
";

        static String multiplePredefinedTypeFixed = @"
using System;

namespace SomeTests
{
    public class BasicClass
    {
        private Object o;
        private Int32 i;

        public void SomeWork(String message)
        {
            i += 1;
            lock(o)
            {
                Console.WriteLine(message);
            }
        }
    }
}
";

        [TestMethod]
        [TestCategory("AvoidPreDefinedTypesUnitTests")]
        public void TestSinglePredefinedType()
        {
            var expected = new DiagnosticResult
            {
                Id = AvoidPreDefinedTypesAnalyzer.DiagnosticId,
                Message = String.Format(AvoidPreDefinedTypesAnalyzer.MessageFormat, "string", "String"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 8, 30)
                        }
            };

            VerifyCSharpDiagnostic(singlePredefinedType, expected);
        }

        [TestMethod]
        [TestCategory("AvoidPreDefinedTypesUnitTests")]
        public void TestSinglePredefindedFixed()
        {
            VerifyCSharpFix(singlePredefinedType, singlePredefinedTypeFixed);
        }

        [TestMethod]
        [TestCategory("AvoidPreDefinedTypesUnitTests")]
        public void TestMultiplePredefinedType()
        {
            var expected = new DiagnosticResult
            {
                Id = AvoidPreDefinedTypesAnalyzer.DiagnosticId,
                Message = String.Format(AvoidPreDefinedTypesAnalyzer.MessageFormat, "object", "Object"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 17)
                    }
            };

            var expected2 = new DiagnosticResult
            {
                Id = AvoidPreDefinedTypesAnalyzer.DiagnosticId,
                Message = String.Format(AvoidPreDefinedTypesAnalyzer.MessageFormat, "int", "Int32"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 17)
                    }
            };

            var expected3 = new DiagnosticResult
            {
                Id = AvoidPreDefinedTypesAnalyzer.DiagnosticId,
                Message = String.Format(AvoidPreDefinedTypesAnalyzer.MessageFormat, "string", "String"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 30)
                    }
            };

            VerifyCSharpDiagnostic(multiplePredefinedType, expected, expected2, expected3);
        }

        [TestMethod]
        [TestCategory("AvoidPreDefinedTypesUnitTests")]
        public void TestMultiplePredefindedFixed()
        {
            VerifyCSharpFix(multiplePredefinedType, multiplePredefinedTypeFixed);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new AvoidPredefinedTypesCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new AvoidPreDefinedTypesAnalyzer();
        }

    }
}
