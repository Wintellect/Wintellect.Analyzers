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
    public class CatchBlockShouldRethrowUnitTests : CodeFixVerifier
    {
        private const String CatchBlockShouldRethrowId = "Wintellect014";
        private const String CatchBlockShouldRethrowMessageFormat = "Catch blocks should rethrow or throw";

        [TestMethod]
        [TestCategory("CatchBlockShouldRethrowUnitTests")]
        public void BadEatExceptionTest()
        {
            const String eatTest = @"
using System;
using System.IO;

namespace Tests
{
    class EatException
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(""Args = {0}"", args.Length);
            }
            catch (FileNotFoundException)
            {
            }
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = CatchBlockShouldRethrowId,
                Message = CatchBlockShouldRethrowMessageFormat,
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 15, 13)
                        }
            };

            VerifyCSharpDiagnostic(eatTest, expected);
        }

        [TestMethod]
        [TestCategory("CatchBlockShouldRethrowUnitTests")]
        public void BadMultipleEatExceptionTest()
        {
            const String eatTest = @"
using System;
using System.IO;

namespace Tests
{
    class EatException
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(""Args = {0}"", args.Length);
            }
            catch (FileNotFoundException)
            {
            }
            catch
            {
            }
        }
    }
}";

            DiagnosticResult[] results = new DiagnosticResult[2];

            results[0] = new DiagnosticResult
            {
                Id = CatchBlockShouldRethrowId,
                Message = CatchBlockShouldRethrowMessageFormat,
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 15, 13)
                        }
            };

            results[1] = new DiagnosticResult
            {
                Id = CatchBlockShouldRethrowId,
                Message = CatchBlockShouldRethrowMessageFormat,
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 18, 13)
                        }
            };

            VerifyCSharpDiagnostic(eatTest, results);
        }

        [TestMethod]
        [TestCategory("CatchBlockShouldRethrowUnitTests")]
        public void GoodRethrowTest()
        {
            const String goodThrow = @"
using System;
using System.IO;

namespace Tests
{
    class EatException
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(""Args = {0}"", args.Length);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(""Got an exception"");
                throw;
            }
        }
    }
}";
            VerifyCSharpDiagnostic(goodThrow);
        }

        [TestMethod]
        [TestCategory("CatchBlockShouldRethrowUnitTests")]
        public void GoodMultipleRethrowTest()
        {
            const String goodThrow = @"
using System;
using System.IO;

namespace Tests
{
    class EatException
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(""Args = {0}"", args.Length);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(""Got an exception"");
                throw;
            }
            catch
            {
                throw;
            }
        }
    }
}";
            VerifyCSharpDiagnostic(goodThrow);
        }

        [TestMethod]
        [TestCategory("CatchBlockShouldRethrowUnitTests")]
        public void BadOnePathEatsException()
        {
            const String eatTest = @"
using System;
using System.IO;

namespace Tests
{
    class EatException
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(""Args = {0}"", args.Length);
            }
            catch (FileNotFoundException)
            {
                if (args.Length < 5)
                {
                    throw;
                }
            }
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = CatchBlockShouldRethrowId,
                Message = CatchBlockShouldRethrowMessageFormat,
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 15, 13)
                        }
            };

            VerifyCSharpDiagnostic(eatTest, expected);
        }

        [TestMethod]
        [TestCategory("CatchBlockShouldRethrowUnitTests")]
        public void BadOnePathReturns()
        {
            const String eatTest = @"
using System;
using System.IO;

namespace Tests
{
    class EatException
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(""Args = {0}"", args.Length);
            }
            catch (FileNotFoundException)
            {
                if (args.Length < 5)
                {
                    return;
                }
                else
                {
                    throw;
                }
            }
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = CatchBlockShouldRethrowId,
                Message = CatchBlockShouldRethrowMessageFormat,
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 15, 13)
                        }
            };

            VerifyCSharpDiagnostic(eatTest, expected);
        }

        [TestMethod]
        [TestCategory("CatchBlockShouldRethrowUnitTests")]
        public void BadMultipleMethodsOneBadOneError()
        {
            const String eatTest = @"
using System;
using System.IO;

namespace Tests
{
    class EatException
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(""Args = { 0}"", args.Length);
            }
            catch (FileNotFoundException)
            {
                if (args.Length < 5)
                {
                    return;
                }
                else
                {
                    throw;
                }
            }
        }

        static void AnotherMethodWithError()
        {
            try
            {
                Console.Write(""Hello"");
            }
            catch
            {
                Console.Write(""{0}"", i);
            }
        }
    }
}
";

            var expected = new DiagnosticResult
            {
                Id = CatchBlockShouldRethrowId,
                Message = CatchBlockShouldRethrowMessageFormat,
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 15, 13)
                        }
            };

            VerifyCSharpDiagnostic(eatTest, expected);
        }
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new CatchBlocksShouldRethrowAnalyzer();
        }

    }
}
