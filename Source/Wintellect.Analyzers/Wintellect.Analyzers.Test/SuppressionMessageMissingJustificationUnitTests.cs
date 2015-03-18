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
    public class SuppressionMessageJustificationUnitTests : CodeFixVerifier
    {

        private const String SuppressionMessageJustificationId = "Wintellect011";
        private const String SuppressionMessageJustificationMessageFormat = "The SuppressionMessage on '{0}' needs the Justification parameter filled out";

        [TestMethod]
        [TestCategory("SuppressionMessageJustificationUnitTests")]
        public void GeneratedCodeAttributeOnMethod()
        {
            var test = @"
using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;

namespace SomeTests
{
	public class BasicClass
	{
        [GeneratedCode ( ""Fake"" , ""1.0"" )]
        [SuppressMessage(""Microsoft.Performance"",
                            ""CA1822:MarkMembersAsStatic"")]
        [SuppressMessage(""Microsoft.Naming"",
                            ""CA1709:IdentifiersShouldBeCasedCorrectly"",
                            MessageId = ""Xo"")]
        public void XoXoX()
        {
        }
    }
    }
}
";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("SuppressionMessageJustificationUnitTests")]
        public void GeneratedCodeFullAttributeOnMethod()
        {
            var test = @"
using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;

namespace SomeTests
{
	public class BasicClass
	{
        [System.CodeDom.Compiler.GeneratedCode ( ""Fake"" , ""1.0"" )]
        [SuppressMessage(""Microsoft.Performance"",
                            ""CA1822:MarkMembersAsStatic"")]
        [SuppressMessage(""Microsoft.Naming"",
                            ""CA1709:IdentifiersShouldBeCasedCorrectly"",
                            MessageId = ""Xo"")]
        public void XoXoX()
        {
        }
    }
}
";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("SuppressionMessageJustificationUnitTests")]
        public void DebuggerNonUserCodeAttributeOnMethod()
        {
            var test = @"
using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

namespace SomeTests
{
	public class BasicClass
	{
        [DebuggerNonUserCode]
        [SuppressMessage(""Microsoft.Naming"",
                            ""CA1709:IdentifiersShouldBeCasedCorrectly"",
                            MessageId = ""Xo"")]
        public void XoXoX()
        {
        }
    }
}
";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("SuppressionMessageJustificationUnitTests")]
        public void GeneratedCodeAttributeOnClass()
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
        [SuppressMessage(""Microsoft.Performance"",
                            ""CA1822:MarkMembersAsStatic"")]
        [SuppressMessage(""Microsoft.Naming"",
                            ""CA1709:IdentifiersShouldBeCasedCorrectly"",
                            MessageId = ""Xo"")]
        public void XoXoX()
        {
        }
    }
    }
}
";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("SuppressionMessageJustificationUnitTests")]
        public void DebuggerNonUserCodeAttributeOnClass()
        {
            var test = @"
using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

namespace SomeTests
{
    [DebuggerNonUserCode]
	public class BasicClass
	{
        [SuppressMessage(""Microsoft.Performance"",
                            ""CA1822:MarkMembersAsStatic"")]
        [SuppressMessage(""Microsoft.Naming"",
                            ""CA1709:IdentifiersShouldBeCasedCorrectly"",
                            MessageId = ""Xo"")]
        public void XoXoX()
        {
        }
    }
    }
}
";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("SuppressionMessageJustificationUnitTests")]
        public void GeneratedCodeAttributeOnAssembly()
        {
            var test = @"
using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;

[assembly: GeneratedCode ( ""Fake"" , ""1.0"" )]
namespace SomeTests
{
	public class BasicClass
	{
        [SuppressMessage(""Microsoft.Performance"",
                            ""CA1822:MarkMembersAsStatic"")]
        [SuppressMessage(""Microsoft.Naming"",
                            ""CA1709:IdentifiersShouldBeCasedCorrectly"",
                            MessageId = ""Xo"")]
        public void XoXoX()
        {
        }
    }
    }
}
";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("SuppressionMessageJustificationUnitTests")]
        public void DebuggerNonUserCodeAttributeOnAssembly()
        {
            var test = @"
using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

[assembly: DebuggerNonUserCode]
namespace SomeTests
{
	public class BasicClass
	{
        [SuppressMessage(""Microsoft.Performance"",
                            ""CA1822:MarkMembersAsStatic"")]
        [SuppressMessage(""Microsoft.Naming"",
                            ""CA1709:IdentifiersShouldBeCasedCorrectly"",
                            MessageId = ""Xo"")]
        public void XoXoX()
        {
        }
    }
    }
}
";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("SuppressionMessageJustificationUnitTests")]
        public void MethodNoJustification()
        {
            var test = @"
using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;

namespace SomeTests
{
	public class BasicClass
	{
        [SuppressMessage(""Microsoft.Naming"",
                            ""CA1709:IdentifiersShouldBeCasedCorrectly"",
                            MessageId = ""Xo"")]
        public void XoXoX()
        {
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = SuppressionMessageJustificationId,
                Message = String.Format(SuppressionMessageJustificationMessageFormat, "XoXoX"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 13, 21)
                }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        [TestCategory("SuppressionMessageJustificationUnitTests")]
        public void MethodEmptyJustification()
        {
            var test = @"
using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;

namespace SomeTests
{
	public class BasicClass
	{
        [SuppressMessage(""Microsoft.Naming"",
                            ""CA1709:IdentifiersShouldBeCasedCorrectly"",
                            MessageId = ""Xo"",
                            Justification = """")]
        public void XoXoX()
        {
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = SuppressionMessageJustificationId,
                Message = String.Format(SuppressionMessageJustificationMessageFormat, "XoXoX"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 14, 21)
                }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        [TestCategory("SuppressionMessageJustificationUnitTests")]
        public void MethodEmptyJustificationFullName()
        {
            var test = @"
using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;

namespace SomeTests
{
	public class BasicClass
	{
        [System.Diagnostics.CodeAnalysis.SuppressMessage(""Microsoft.Naming"",
                            ""CA1709:IdentifiersShouldBeCasedCorrectly"",
                            MessageId = ""Xo"",
                            Justification = """")]
        public void XoXoX()
        {
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = SuppressionMessageJustificationId,
                Message = String.Format(SuppressionMessageJustificationMessageFormat, "XoXoX"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 14, 21)
                }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        [TestCategory("SuppressionMessageJustificationUnitTests")]
        public void MethodGoodJustification()
        {
            var test = @"
using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;

namespace SomeTests
{
	public class BasicClass
	{
        [SuppressMessage(""Microsoft.Naming"",
                            ""CA1709:IdentifiersShouldBeCasedCorrectly"",
                            MessageId = ""Xo"",
                            Justification = ""Good reason why"")]
        public void XoXoX()
        {
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("SuppressionMessageJustificationUnitTests")]
        public void FieldMissingJustification()
        {
            var test = @"
using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;

namespace SomeTests
{
	public class BasicClass
	{
        /// <summary>
        /// 
        /// </summary>
        [SuppressMessage(""Microsoft.Design"",
                           ""CA1051:DoNotDeclareVisibleInstanceFields"",
                            Justification = ""Good reason why"")]
        public String fieldMissingJustifixxY = String.Empty;
    }
}
";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("SuppressionMessageJustificationUnitTests")]
        public void PropertyMissingJustification()
        {
            var test = @"
using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;

namespace SomeTests
{
	public class BasicClass
	{
        /// <summary>
        /// Found....
        /// </summary>
        [SuppressMessage(""Microsoft.Usage"",
                           ""CA1801:ReviewUnusedParameters"",
                           MessageId = ""value"")]
        public String PropertyMissingJustification
        {
            get { return fieldMissingJustifixxY; }
            set { value = fieldMissingJustifixxY; }
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = SuppressionMessageJustificationId,
                Message = String.Format(SuppressionMessageJustificationMessageFormat, "PropertyMissingJustification"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 16, 23)
                }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        [TestCategory("SuppressionMessageJustificationUnitTests")]
        public void PropertyGoodJustification()
        {
            var test = @"
using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;

namespace SomeTests
{
	public class BasicClass
	{
        /// <summary>
        /// Found....
        /// </summary>
        [SuppressMessage(""Microsoft.Usage"",
                           ""CA1801:ReviewUnusedParameters"",
                           MessageId = ""value"",
                           Justification = ""Good reason why!"")]
        public String PropertyMissingJustification
        {
            get { return fieldMissingJustifixxY; }
            set { value = fieldMissingJustifixxY; }
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("SuppressionMessageJustificationUnitTests")]
        public void ClassGoodJustification()
        {
            var test = @"
using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;

namespace SomeTests
{
    [SuppressMessage(""Microsoft.Usage"",
                        ""CA1801:ReviewUnusedParameters"",
                        MessageId = ""value"",
                        Justification = ""Good reason why!"")]
	public class BasicClass
	{
        /// <summary>
        /// Found....
        /// </summary>
        public String PropertyMissingJustification
        {
            get { return fieldMissingJustifixxY; }
            set { value = fieldMissingJustifixxY; }
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("SuppressionMessageJustificationUnitTests")]
        public void ClassyMissingJustification()
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
        /// <summary>
        /// Found....
        /// </summary>
        public String PropertyMissingJustification
        {
            get { return fieldMissingJustifixxY; }
            set { value = fieldMissingJustifixxY; }
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = SuppressionMessageJustificationId,
                Message = String.Format(SuppressionMessageJustificationMessageFormat, "BasicClass"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 11, 15)
                }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        [TestCategory("SuppressionMessageJustificationUnitTests")]
        public void MethodPendingJustification()
        {
            var test = @"
using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;

namespace SomeTests
{
	public class BasicClass
	{
        [SuppressMessage(""Microsoft.Naming"",
                            ""CA1709:IdentifiersShouldBeCasedCorrectly"",
                            MessageId = ""Xo"",
                            Justification = ""<Pending>"")]
        public void XoXoX()
        {
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = SuppressionMessageJustificationId,
                Message = String.Format(SuppressionMessageJustificationMessageFormat, "XoXoX"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 14, 21)
                }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        [TestCategory("SuppressionMessageJustificationUnitTests")]
        public void MethodPendingJustificationFullName()
        {
            var test = @"
using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;

namespace SomeTests
{
	public class BasicClass
	{
        [System.Diagnostics.CodeAnalysis.SuppressMessage(""Microsoft.Naming"",
                            ""CA1709:IdentifiersShouldBeCasedCorrectly"",
                            MessageId = ""Xo"",
                            Justification = ""<Pending>"")]
        public void XoXoX()
        {
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = SuppressionMessageJustificationId,
                Message = String.Format(SuppressionMessageJustificationMessageFormat, "XoXoX"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 14, 21)
                }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SuppressionMessageMissingJustificationAnalyzer();
        }
    }
}
