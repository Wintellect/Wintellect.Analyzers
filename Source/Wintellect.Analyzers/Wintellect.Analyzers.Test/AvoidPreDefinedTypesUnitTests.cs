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
        const String AvoidPreDefinedTypesAnalyzeId = "Wintellect004";
        const String AvoidPreDefinedTypesAnalyzerMessageFormat = "Convert '{0}' to the explicit type '{1}'";

        [TestMethod]
        [TestCategory("AvoidPreDefinedTypesUnitTests")]
        public void TestSinglePredefinedType()
        {
            var expected = new DiagnosticResult
            {
                Id = AvoidPreDefinedTypesAnalyzeId,
                Message = String.Format(AvoidPreDefinedTypesAnalyzerMessageFormat, "string", "String"),
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
        public void TestUintPredefinedType()
        {
            String singleUintPredefinedType = @"
using System;

namespace SomeTests
{
    public class BasicClass
    {
        public void SomeWork(uint message)
        {
            Console.WriteLine(message);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = AvoidPreDefinedTypesAnalyzeId,
                Message = String.Format(AvoidPreDefinedTypesAnalyzerMessageFormat, "uint", "UInt32"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 8, 30)
                        }
            };

            VerifyCSharpDiagnostic(singleUintPredefinedType, expected);
        }

        [TestMethod]
        [TestCategory("AvoidPreDefinedTypesUnitTests")]
        public void TestFixedSinglePredefinedType()
        {
            DiagnosticResult[] expected = new DiagnosticResult[0] ;

            VerifyCSharpDiagnostic(singlePredefinedTypeFixed, expected);
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
            DiagnosticResult[] results = new DiagnosticResult[3];

            results[0] = new DiagnosticResult
            {
                Id = AvoidPreDefinedTypesAnalyzeId,
                Message = String.Format(AvoidPreDefinedTypesAnalyzerMessageFormat, "object", "Object"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 17)
                    }
            };

            results[1] = new DiagnosticResult
            {
                Id = AvoidPreDefinedTypesAnalyzeId,
                Message = String.Format(AvoidPreDefinedTypesAnalyzerMessageFormat, "int", "Int32"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 17)
                    }
            };

            results[2] = new DiagnosticResult
            {
                Id = AvoidPreDefinedTypesAnalyzeId,
                Message = String.Format(AvoidPreDefinedTypesAnalyzerMessageFormat, "string", "String"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 30)
                    }
            };

            VerifyCSharpDiagnostic(multiplePredefinedType, results);
        }

        [TestMethod]
        [TestCategory("AvoidPreDefinedTypesUnitTests")]
        public void TestMultiplePredefindedFixed()
        {
            VerifyCSharpFix(multiplePredefinedType, multiplePredefinedTypeFixed);
        }

        [TestMethod]
        [TestCategory("AvoidPreDefinedTypesUnitTests")]
        public void TestIgnoreGeneratedCode()
        {
            var test = @"
using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;

namespace SomeTests
{
    [GeneratedCode ( ""Fake"" , ""1.0"" )]
	public class BasicClass
	{
        public int XoXoX()
        {
            return 5;
        }
    }
}
";

            DiagnosticResult[] expected = new DiagnosticResult[0];

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        [TestCategory("AvoidPreDefinedTypesUnitTests")]
        public void TestIgnoreFullAttributeCode()
        {
            var test = @"
using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;

namespace SomeTests
{
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
	public class BasicClass
	{
        public int XoXoX()
        {
            return 5;
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("AvoidPreDefinedTypesUnitTests")]
        public void TestIgnoreFullGenCodeAttributeCode()
        {
            var test = @"
using System;

namespace SomeTests
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute(""Fake"", ""1.0"")]
    public class BasicClass
        {
            public int XoXoX()
            {
                return 5;
            }
        }
    }
";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("AvoidPreDefinedTypesUnitTests")]
        public void TestDifferentAttributesCode()
        {
            var test = @"
using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;

namespace SomeTests
{
    [SuppressMessage(""Microsoft.Usage"",
                        ""CA1801:ReviewUnusedParameters"",
                        MessageId = ""value"")]
	public class BasicClass
	{
        public Int32 XoXoX()
        {
            return 5;
        }
    }
}
";

            DiagnosticResult[] expected = new DiagnosticResult[0];

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        [TestCategory("AvoidPreDefinedTypesUnitTests")]
        public void TestDifferentAttributesPredefineInMethodCode()
        {
            var test = @"
using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;

namespace SomeTests
{
    [SuppressMessage(""Microsoft.Usage"",
                        ""CA1801:ReviewUnusedParameters"",
                        MessageId = ""value"")]
	public class BasicClass
	{
        public Int32 XoXoX(Int32 p)
        {
            for(int i = 0; i < p; i++)
            {
                if (i % 2)
                {
                    return 2;
                }
            }
            return 5;
        }
    }
}
";

            var expected = new DiagnosticResult
            {
                Id = AvoidPreDefinedTypesAnalyzeId,
                Message = String.Format(AvoidPreDefinedTypesAnalyzerMessageFormat, "int", "Int32"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 15, 17)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
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
