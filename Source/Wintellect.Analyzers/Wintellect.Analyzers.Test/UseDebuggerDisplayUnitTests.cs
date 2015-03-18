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
    public class UseDebuggerDisplayUnitTests : CodeFixVerifier
    {
        const String UseDebuggerDisplayAttributeAnalyzerId = "Wintellect013";
        const String UseDebuggerDisplayAttributeAnalyzerMessageFormat = "The public class '{0}' does not have a DebuggerDisplay attribute applied";

        [TestMethod]
        [TestCategory("UseDebuggerDisplayUnitTests")]

        public void GoodValueType()
        {
            const String publicTest = @"
        using System;
        using System.Diagnostics;

        namespace ConsoleApplication1
        {
            [DebuggerDisplay(""xyzzY"")]
            public struct MyStructName
            {
            }

            [DebuggerDisplay(""xyzzY"")]
            public enum MyEnum
            {
            }

        }";

            VerifyCSharpDiagnostic(publicTest);
        }

        [TestMethod]
        [TestCategory("UseDebuggerDisplayUnitTests")]
        public void GoodPrivateClass()
        {
            const String publicTest = @"
        using System;
        using System.Diagnostics;

        namespace ConsoleApplication1
        {
            private class MyClassName
            {   
                public MyClassName() {}
            }

        }";

            VerifyCSharpDiagnostic(publicTest);
        }


        [TestMethod]
        [TestCategory("UseDebuggerDisplayUnitTests")]
        public void GoodNoState()
        {
            const String publicTest = @"
        using System;
        using System.Diagnostics;

        namespace ConsoleApplication1
        {
            public class MyClassName
            {   
                public MyClassName() {}
            }

        }";

            VerifyCSharpDiagnostic(publicTest);
        }

        [TestMethod]
        [TestCategory("UseDebuggerDisplayUnitTests")]
        public void GoodOverridesToString()
        {
            const String publicTest = @"
        using System;
        using System.Diagnostics;

        namespace ConsoleApplication1
        {
            public class MyClassName
            {   
                Int32 fakeData;
                public MyClassName(Int32 data) { fakeData = data; }

                public override String ToString()
                {
                    return base.ToString();
                }
            }

        }";

            VerifyCSharpDiagnostic(publicTest);
        }

        [TestMethod]
        [TestCategory("UseDebuggerDisplayUnitTests")]
        public void GoodHasDebuggerDisplay()
        {
            const String publicTest = @"
        using System;
        using System.Diagnostics;

        namespace ConsoleApplication1
        {
            [DebuggerDisplay(""Important stuff here"")]
            public class MyClassName
            {   
                Int32 fakeData;
                public MyClassName(Int32 data) { fakeData = data; }
            }

        }";

            VerifyCSharpDiagnostic(publicTest);
        }

        [TestMethod]
        [TestCategory("UseDebuggerDisplayUnitTests")]
        public void BadNoDebuggerDisplay()
        {
            const String publicTest = @"
using System;
using System.Diagnostics;

namespace ConsoleApplication1
{
    public class MyClassName
    {   
        Int32 fakeData;
        public MyClassName(Int32 data) { fakeData = data; }
    }

}";
            var expected = new DiagnosticResult
            {
                Id = UseDebuggerDisplayAttributeAnalyzerId,
                Message = String.Format(UseDebuggerDisplayAttributeAnalyzerMessageFormat, "MyClassName"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 7, 18) }
            };


            VerifyCSharpDiagnostic(publicTest, expected);
        }

        [TestMethod]
        [TestCategory("UseDebuggerDisplayUnitTests")]
        public void BadCodeTest()
        {
            var test = @"
using System;

namespace ConsoleApplication1
{
    public class B : IEnumerable
    {
        Int32 fakeData2;
        public String FakeProperty2
        {
            get;
            set;
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("UseDebuggerDisplayUnitTests")]
        public void EnumerableFixTest()
        {
            var test = @"
using System;
using System.Collections;

namespace ConsoleApplication1
{
    public class B : IEnumerable
    {
        Int32 fakeData2;
        public String FakeProperty2
        {
            get;
            set;
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}";

            var fixtest = @"
using System;
using System.Collections;
using System.Diagnostics;

namespace ConsoleApplication1
{
    // TODO: Change the automatically inserted DebuggerDisplay string from Wintellect.Analyzers
    [DebuggerDisplay(""Count={Count()}"")]
    public class B : IEnumerable
    {
        Int32 fakeData2;
        public String FakeProperty2
        {
            get;
            set;
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        [TestCategory("UseDebuggerDisplayUnitTests")]
        public void EnumerableTwoDeepFixTest()
        {
            var test = @"
using System;
using System.Collections;

namespace ConsoleApplication1
{
    public class B : IEnumerable
    {
        Int32 fakeData2;
        public String FakeProperty2
        {
            get;
            set;
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
    public class A : B
    {
        Int32 fakeData3;
        public String FakeProperty3
        {
            get;
            set;
        }
    }
}";

            var fixtest = @"
using System;
using System.Collections;
using System.Diagnostics;

namespace ConsoleApplication1
{
    // TODO: Change the automatically inserted DebuggerDisplay string from Wintellect.Analyzers
    [DebuggerDisplay(""Count={Count()}"")]
    public class B : IEnumerable
    {
        Int32 fakeData2;
        public String FakeProperty2
        {
            get;
            set;
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
    // TODO: Change the automatically inserted DebuggerDisplay string from Wintellect.Analyzers
    [DebuggerDisplay(""Count={Count()}"")]
    public class A : B
    {
        Int32 fakeData3;
        public String FakeProperty3
        {
            get;
            set;
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        [TestCategory("UseDebuggerDisplayUnitTests")]
        public void EnumerableMultipleInheritanceFixTest()
        {
            var test = @"
using System;
using System.Collections;

namespace ConsoleApplication1
{
    public class B : IEnumerable, IComparable
    {
        Int32 fakeData2;
        public String FakeProperty2
        {
            get;
            set;
        }

        public Int32 CompareTo(Object obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}";

            var fixtest = @"
using System;
using System.Collections;
using System.Diagnostics;

namespace ConsoleApplication1
{
    // TODO: Change the automatically inserted DebuggerDisplay string from Wintellect.Analyzers
    [DebuggerDisplay(""Count={Count()}"")]
    public class B : IEnumerable, IComparable
    {
        Int32 fakeData2;
        public String FakeProperty2
        {
            get;
            set;
        }

        public Int32 CompareTo(Object obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        [TestCategory("UseDebuggerDisplayUnitTests")]
        public void SimpleFixTest()
        {
            var test = @"
using System;

namespace ConsoleApplication1
{
    public class MyClassName
    {
        Int32 fakeData;
        public String FakePropertyOne
        {
            get;
            set;
        }

        public String FakePropertyTwo
        {
            get;
            set;
        }

        public MyClassName(Int32 data)
        {
            fakeData = data;
        }
    }
}";
            var fixtest = @"
using System;
using System.Diagnostics;

namespace ConsoleApplication1
{
    // TODO: Change the automatically inserted DebuggerDisplay string from Wintellect.Analyzers
    [DebuggerDisplay(""FakePropertyOne={FakePropertyOne} FakePropertyTwo={FakePropertyTwo}"")]
    public class MyClassName
    {
        Int32 fakeData;
        public String FakePropertyOne
        {
            get;
            set;
        }

        public String FakePropertyTwo
        {
            get;
            set;
        }

        public MyClassName(Int32 data)
        {
            fakeData = data;
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        [TestCategory("UseDebuggerDisplayUnitTests")]
        public void OnePropOneFieldFixTest()
        {
            var test = @"
using System;

namespace ConsoleApplication1
{
    public class MyClassName
    {
        Int32 fakeData;
        public String FakePropertyOne
        {
            get;
            set;
        }

        public MyClassName(Int32 data)
        {
            fakeData = data;
        }
    }
}";
            var fixtest = @"
using System;
using System.Diagnostics;

namespace ConsoleApplication1
{
    // TODO: Change the automatically inserted DebuggerDisplay string from Wintellect.Analyzers
    [DebuggerDisplay(""FakePropertyOne={FakePropertyOne} fakeData={fakeData}"")]
    public class MyClassName
    {
        Int32 fakeData;
        public String FakePropertyOne
        {
            get;
            set;
        }

        public MyClassName(Int32 data)
        {
            fakeData = data;
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        [TestCategory("UseDebuggerDisplayUnitTests")]
        public void MultipleAttributeFixTest()
        {
            var test = @"
using System;

namespace ConsoleApplication1
{
    [Serializable()]
    public class MyClassName
    {
        Int32 fakeData;
        public String FakePropertyOne
        {
            get;
            set;
        }

        public MyClassName(Int32 data)
        {
            fakeData = data;
        }
    }
}";
            var fixtest = @"
using System;
using System.Diagnostics;

namespace ConsoleApplication1
{
    [Serializable()]
    // TODO: Change the automatically inserted DebuggerDisplay string from Wintellect.Analyzers
    [DebuggerDisplay(""FakePropertyOne={FakePropertyOne} fakeData={fakeData}"")]
    public class MyClassName
    {
        Int32 fakeData;
        public String FakePropertyOne
        {
            get;
            set;
        }

        public MyClassName(Int32 data)
        {
            fakeData = data;
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        [TestCategory("UseDebuggerDisplayUnitTests")]
        public void ThreeFieldFixTest()
        {
            var test = @"
using System;

namespace ConsoleApplication1
{
    public class MyClassName
    {
        Int32 fakeData;
        Int32 fakeData2;
        Int32 fakeData3;
        public MyClassName(Int32 data)
        {
            fakeData = data;
            fakeData2 = data;
            fakeData3 = data;
        }
    }
}";
            var fixtest = @"
using System;
using System.Diagnostics;

namespace ConsoleApplication1
{
    // TODO: Change the automatically inserted DebuggerDisplay string from Wintellect.Analyzers
    [DebuggerDisplay(""fakeData={fakeData} fakeData2={fakeData2}"")]
    public class MyClassName
    {
        Int32 fakeData;
        Int32 fakeData2;
        Int32 fakeData3;
        public MyClassName(Int32 data)
        {
            fakeData = data;
            fakeData2 = data;
            fakeData3 = data;
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        [TestCategory("UseDebuggerDisplayUnitTests")]
        public void MultipleAttributesWithCommentsTest()
        {
            var test = @"
using System;

namespace ConsoleApplication1
{
    // A comment on serializable. Does it stay?
    [Serializable]
    public class MyClassName
    {
        Int32 fakeData;
        public String FakePropertyOne
        {
            get;
            set;
        }
        public String FakePropertyTwo
        {
            get;
            set;
        }
    }
}";
            var fixtest = @"
using System;
using System.Diagnostics;

namespace ConsoleApplication1
{
    // A comment on serializable. Does it stay?
    [Serializable]
    // TODO: Change the automatically inserted DebuggerDisplay string from Wintellect.Analyzers
    [DebuggerDisplay(""FakePropertyOne={FakePropertyOne} FakePropertyTwo={FakePropertyTwo}"")]
    public class MyClassName
    {
        Int32 fakeData;
        public String FakePropertyOne
        {
            get;
            set;
        }
        public String FakePropertyTwo
        {
            get;
            set;
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new UseDebuggerDisplayAttributeCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new UseDebuggerDisplayAttributeAnalyzer();
        }
    }
}
