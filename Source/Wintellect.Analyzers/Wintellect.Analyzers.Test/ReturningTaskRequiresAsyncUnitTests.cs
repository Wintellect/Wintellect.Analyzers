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
    public class ReturningTaskRequiresAsyncTests : CodeFixVerifier
    {
        static String nonGenericTaskReturn = @"
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace NonGenericTask
{
    public class NonGenericTask
    {
        private Task GenericOperation()
        {
            return null;
        }
    }
}
";

        static String nonGenericTaskReturnFixed = @"
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace NonGenericTask
{
    public class NonGenericTask
    {
        private Task GenericOperationAsync()
        {
            return null;
        }
    }
}
";

        static String genericTaskReturn = @"
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace NonGenericTask
{
    public class GenericTask
    {
        private Task<String> GenericOperation()
        {
            return null;
        }
    }
}
";

        static String genericTaskReturnFixed = @"
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace NonGenericTask
{
    public class GenericTask
    {
        private Task<String> GenericOperationAsync()
        {
            return null;
        }
    }
}
";

        private const String ReturningTaskRequiresAsyncAnalyzerId = "Wintellect001";
        private const String ReturningTaskRequiresAsyncAnalyzerMessageFormat = "Method name {0} should be renamed {0}Async";

        [TestMethod]
        [TestCategory("ReturningTaskRequiresAsyncTests")]
        public void TestNonGenericMethod()
        {
            var expected = new DiagnosticResult
            {
                Id = ReturningTaskRequiresAsyncAnalyzerId,
                Message = String.Format(ReturningTaskRequiresAsyncAnalyzerMessageFormat, "GenericOperation"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 12, 22)
                        }
            };

            VerifyCSharpDiagnostic(nonGenericTaskReturn, expected);
        }

        [TestMethod]
        [TestCategory("ReturningTaskRequiresAsyncTests")]
        public void TestGenericMethod()
        {
            var expected = new DiagnosticResult
            {
                Id = ReturningTaskRequiresAsyncAnalyzerId,
                Message = String.Format(ReturningTaskRequiresAsyncAnalyzerMessageFormat, "GenericOperation"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 12, 30)
                        }
            };

            VerifyCSharpDiagnostic(genericTaskReturn, expected);
        }

        [TestMethod]
        [TestCategory("ReturningTaskRequiresAsyncTests")]
        public void TestNonGenericMethodFix()
        {
            VerifyCSharpFix(nonGenericTaskReturn, nonGenericTaskReturnFixed);
        }

        [TestMethod]
        [TestCategory("ReturningTaskRequiresAsyncTests")]
        public void TestGenericMethodFix()
        {
            VerifyCSharpFix(genericTaskReturn, genericTaskReturnFixed);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new ReturningTaskRequiresAsyncCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ReturningTaskRequiresAsyncAnalyzer();
        }
    }
}