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
    public class ClassesShouldBeSealedUnitTests : CodeFixVerifier
    {
        private const String ClassesShouldBeSealedId = "Wintellect012";
        private const String ClassesShouldBeSealedMissingMessageFormat = "The class '{0}' should be declared sealed if this is a newly written class";

        const String privateTest = @"
using System;
using System.Diagnostics;

namespace ConsoleApplication1
{
    private class MyClassName
    {   
        public MyClassName() {}
    }

    struct MyStructName
    {
    }

}";

        const String privateFixTest = @"
using System;
using System.Diagnostics;

namespace ConsoleApplication1
{
    private sealed class MyClassName
    {   
        public MyClassName() {}
    }

    struct MyStructName
    {
    }

}";

        const String publicTest = @"
using System;
using System.Diagnostics;

namespace ConsoleApplication1
{
    public class MyClassName
    {   
        public MyClassName() {}
    }

    struct MyStructName
    {
    }

}";

        const String publicFixTest = @"
using System;
using System.Diagnostics;

namespace ConsoleApplication1
{
    public sealed class MyClassName
    {   
        public MyClassName() {}
    }

    struct MyStructName
    {
    }

}";

        [TestMethod]
        [TestCategory("ClassesShouldBeSealedUnitTests")]
        public void CheckPrivateIsSealed()
        {
            var expected = new DiagnosticResult
            {
                Id = ClassesShouldBeSealedId,
                Message = String.Format(ClassesShouldBeSealedMissingMessageFormat, "MyClassName"),
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 7, 19)
                        }
            };

            VerifyCSharpDiagnostic(privateTest, expected);
        }

        [TestMethod]
        [TestCategory("ClassesShouldBeSealedUnitTests")]
        public void FixPrivateIsSealed()
        {
            VerifyCSharpFix(privateTest, privateFixTest);
        }

        [TestMethod]
        [TestCategory("ClassesShouldBeSealedUnitTests")]
        public void CheckPublicIsSealed()
        {
            var expected = new DiagnosticResult
            {
                Id = ClassesShouldBeSealedId,
                Message = String.Format(ClassesShouldBeSealedMissingMessageFormat, "MyClassName"),
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 7, 18)
                        }
            };

            VerifyCSharpDiagnostic(publicTest, expected);
        }

        [TestMethod]
        [TestCategory("ClassesShouldBeSealedUnitTests")]
        public void FixPublicIsSealed()
        {
            VerifyCSharpFix(publicTest, publicFixTest);
        }

        [TestMethod]
        [TestCategory("ClassesShouldBeSealedUnitTests")]
        public void TestGeneratedCodeNotSealed()
        {
            String thisTest = @"
using System;
using System.Diagnostics;
using System.CodeDom.Compiler;

namespace ConsoleApplication1
{
    [GeneratedCode ( ""Fake"" , ""1.0"" )]
    private class ShouldBeIgnored
    {   
        public MyClassName() {}
    }

    struct MyStructName
    {
    }

}";
            VerifyCSharpDiagnostic(thisTest);
        }

        [TestMethod]
        [TestCategory("ClassesShouldBeSealedUnitTests")]
        public void TestAbstractClassNotSealed()
        {
            String thisTest = @"
using System;
using System.Diagnostics;
using System.CodeDom.Compiler;

namespace ConsoleApplication1
{
    private abstract class ShouldBeIgnored
    {   
        public MyClassName() {}

         abstract public int SomeWork();
    }

    struct MyStructName
    {
    }

}";
            VerifyCSharpDiagnostic(thisTest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new ClassesShouldBeSealedCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ClassesShouldBeSealedAnalyzer();
        }
    }
}
