/*------------------------------------------------------------------------------
Wintellect.Analyzers - .NET Compiler Platform ("Roslyn") Analyzers and CodeFixes
Copyright (c) Wintellect. All rights reserved
Licensed under the Apache License, Version 2.0
See License.txt in the project root for license information
------------------------------------------------------------------------------*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;

namespace Wintellect.Analyzers.Test
{
    [TestClass]
    public class AssemblyAttributesUnitTests : CodeFixVerifier
    {
        const String AssembliesHaveCompanyAttributeAnalyzerId = "Wintellect006";
        const String AssembliesHaveCompanyAttributeAnalyzerMessageFormat = "Add a filled out AssemblyCompanyAttribute to the assembly properties";
        const String AssembliesHaveCopyrightAttributeAnalyzerId = "Wintellect007";
        const String AssembliesHaveCopyrightAttributeAnalyzerMessageFormat = "Add a filled out AssemblyCopyrightAttribute to the assembly properties";
        const String AssembliesHaveDescriptionAttributeAnalyzerId = "Wintellect008";
        const String AssembliesHaveDescriptionAttributeAnalyzerMessageFormat = "Add a filled out AssemblyDescriptionAttribute to the assembly properties";
        const String AssembliesHaveTitleAttributeAnalyzerId = "Wintellect009";
        const String AssembliesHaveTitleAttributeAnalyzerMessageFormat = "Add a filled out AssemblyTitleAttribute to the assembly properties";

        [TestMethod]
        [TestCategory("AssemblyAttributeUnitTests")]
        public void TestCSharpAtrributesNonePresent()
        {
            var expected = new DiagnosticResult[4];
            expected[0] = CompanyAttributeResult;
            expected[1] = CopyrightAttributeResult;
            expected[2] = DescriptionAttributeResult;
            expected[3] = TitleAttributeResult;

            VerifyCSharpDiagnostic(@"
using System;
using System.Reflection;

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
",          expected);
        }

        [TestMethod]
        [TestCategory("AssemblyAttributeUnitTests")]
        public void TestCSharpAtrributesPresentButEmpty()
        {
            var expected = new DiagnosticResult[4];
            expected[0] = CompanyAttributeResult;
            expected[1] = CopyrightAttributeResult;
            expected[2] = DescriptionAttributeResult;
            expected[3] = TitleAttributeResult;

            VerifyCSharpDiagnostic(@"
using System;
using System.Reflection;

[assembly: AssemblyTitle("""")]
[assembly: AssemblyDescription("""")]
[assembly: AssemblyCompany("""")]
[assembly: AssemblyCopyright("""")]

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
", expected);
        }

        [TestMethod]
        [TestCategory("AssemblyAttributeUnitTests")]
        public void TestCSharpAtrributesOnlyTitleFilled()
        {
            var expected = new DiagnosticResult[3];
            expected[0] = CompanyAttributeResult;
            expected[1] = CopyrightAttributeResult;
            expected[2] = DescriptionAttributeResult;

            VerifyCSharpDiagnostic(@"
using System;
using System.Reflection;

[assembly: AssemblyTitle(""De oppresso liber"")]
[assembly: AssemblyDescription("""")]
[assembly: AssemblyCompany("""")]
[assembly: AssemblyCopyright("""")]

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
", expected);
        }

        [TestMethod]
        [TestCategory("AssemblyAttributeUnitTests")]
        public void TestCSharpAtrributesOnlyDescriptionFilled()
        {
            var expected = new DiagnosticResult[3];
            expected[0] = CompanyAttributeResult;
            expected[1] = CopyrightAttributeResult;
            expected[2] = TitleAttributeResult;

            VerifyCSharpDiagnostic(@"
using System;
using System.Reflection;

[assembly: AssemblyTitle("""")]
[assembly: AssemblyDescription(""De oppresso liber"")]
[assembly: AssemblyCompany("""")]
[assembly: AssemblyCopyright("""")]

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
", expected);
        }

        [TestMethod]
        [TestCategory("AssemblyAttributeUnitTests")]
        public void TestCSharpAtrributesOnlyCopyrightFilled()
        {
            var expected = new DiagnosticResult[3];
            expected[0] = CompanyAttributeResult;
            expected[1] = DescriptionAttributeResult;
            expected[2] = TitleAttributeResult;

            VerifyCSharpDiagnostic(@"
using System;
using System.Reflection;

[assembly: AssemblyTitle("""")]
[assembly: AssemblyDescription("""")]
[assembly: AssemblyCompany("""")]
[assembly: AssemblyCopyright(""De oppresso liber"")]

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
", expected);
        }

        [TestMethod]
        [TestCategory("AssemblyAttributeUnitTests")]
        public void TestCSharpAtrributesOnlyCompanyFilled()
        {
            var expected = new DiagnosticResult[3];
            expected[0] = CopyrightAttributeResult;
            expected[1] = DescriptionAttributeResult;
            expected[2] = TitleAttributeResult;

            VerifyCSharpDiagnostic(@"
using System;
using System.Reflection;

[assembly: AssemblyTitle("""")]
[assembly: AssemblyDescription("""")]
[assembly: AssemblyCompany(""De opresson liber"")]
[assembly: AssemblyCopyright("""")]

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
", expected);
        }

        //        [TestMethod]
        //        [TestCategory("AssemblyAttributeUnitTests")]
        //        public void TestVisualBasicZeroAtrributes()
        //        {
        //            var expected = new DiagnosticResult[4];
        //            expected[0] = CompanyAttributeResult;
        //            expected[1] = CopyrightAttributeResult;
        //            expected[2] = DescriptionAttributeResult;
        //            expected[3] = TitleAttributeResult;

        //            VerifyBasicDiagnostic(@"
        //Imports System.Reflection

        //Namespace SomeTests
        //	Public Class BasicClass
        //		Public Sub SomeWork(message As [String])
        //			Console.WriteLine(message)
        //		End Sub
        //	End Class
        //End Namespace", 
        //            expected);
        //        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new AssemblyAttributeAnalyzer();
        }

        protected override DiagnosticAnalyzer GetBasicDiagnosticAnalyzer()
        {
            return new AssemblyAttributeAnalyzer();
        }

        private static DiagnosticResult CompanyAttributeResult = new DiagnosticResult
        {
            Id = AssembliesHaveCompanyAttributeAnalyzerId,
            Message = AssembliesHaveCompanyAttributeAnalyzerMessageFormat,
            Severity = DiagnosticSeverity.Warning,
        };

        private static DiagnosticResult CopyrightAttributeResult = new DiagnosticResult
        {
            Id = AssembliesHaveCopyrightAttributeAnalyzerId,
            Message = AssembliesHaveCopyrightAttributeAnalyzerMessageFormat,
            Severity = DiagnosticSeverity.Warning,
        };

        private static DiagnosticResult DescriptionAttributeResult = new DiagnosticResult
        {
            Id = AssembliesHaveDescriptionAttributeAnalyzerId,
            Message = AssembliesHaveDescriptionAttributeAnalyzerMessageFormat,
            Severity = DiagnosticSeverity.Warning,
        };

        private static DiagnosticResult TitleAttributeResult = new DiagnosticResult
        {
            Id = AssembliesHaveTitleAttributeAnalyzerId,
            Message = AssembliesHaveTitleAttributeAnalyzerMessageFormat,
            Severity = DiagnosticSeverity.Warning,
        };
    }
}
