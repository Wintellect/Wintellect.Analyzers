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
    public class ExceptionDocumentationMissingUnitTests : CodeFixVerifier
    {

        private const String ExceptionDocumentationMissingId = "Wintellect010";
        private const String ExceptionDocumentationMissingMessageFormat = "Document the direct throw of type '{0}' with an <exception> tag";

        [TestMethod]
        [TestCategory("ExceptionDocumentationMissingUnitTests")]
        public void MethodCorrect()
        {
            var test = @"
using System;

namespace SomeTests
{
	public class BasicClass
	{
        /// <summary>
        /// asdasdasd
        /// </summary>
        /// <param name=""message""></param>
        /// <exception cref=""ArgumentException"">WHY~!</exception>
        public void SomeWork(string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                throw new ArgumentException(""message"");
            }
            Console.WriteLine(message);
        }
    }
}
";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("ExceptionDocumentationMissingUnitTests")]
        public void MethodNotDocumented()
        {
            var test = @"
using System;

namespace SomeTests
{
	public class BasicClass
	{
        /// <summary>
        /// asdasdasd
        /// </summary>
        /// <param name=""message""></param>
        public void SomeWork(string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                throw new ArgumentException(""message"");
            }
            Console.WriteLine(message);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = ExceptionDocumentationMissingId,
                Message = String.Format(ExceptionDocumentationMissingMessageFormat, "ArgumentException"),
                Severity = DiagnosticSeverity.Error,
                Locations =new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 16, 17)
                }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        [TestCategory("ExceptionDocumentationMissingUnitTests")]
        public void MethodNoDocComments()
        {
            var test = @"
using System;

namespace SomeTests
{
	public class BasicClass
	{
        public void SomeWork(string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                throw new ArgumentException(""message"");
            }
            Console.WriteLine(message);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = ExceptionDocumentationMissingId,
                Message = String.Format(ExceptionDocumentationMissingMessageFormat, "ArgumentException"),
                Severity = DiagnosticSeverity.Error,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 12, 17)
                }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        [TestCategory("ExceptionDocumentationMissingUnitTests")]
        public void MethodPrivateMethod()
        {
            var test = @"
using System;

namespace SomeTests
{
	public class BasicClass
	{
        /// <summary>
        /// asdasdasd
        /// </summary>
        /// <param name=""message""></param>
        /// <exception cref=""ArgumentException"">WHY~!</exception>
        private void SomeWork(string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                throw new ArgumentException(""message"");
            }
            Console.WriteLine(message);
        }
    }
}
";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("ExceptionDocumentationMissingUnitTests")]
        public void MethodInternalMethod()
        {
            var test = @"
using System;

namespace SomeTests
{
	public class BasicClass
	{
        /// <summary>
        /// asdasdasd
        /// </summary>
        /// <param name=""message""></param>
        /// <exception cref=""ArgumentException"">WHY~!</exception>
        internal void SomeWork(string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                throw new ArgumentException(""message"");
            }
            Console.WriteLine(message);
        }
    }
}
";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("ExceptionDocumentationMissingUnitTests")]
        public void MethodStaticInternalMethod()
        {
            var test = @"
using System;

namespace SomeTests
{
	public class BasicClass
	{
        /// <summary>
        /// asdasdasd
        /// </summary>
        /// <param name=""message""></param>
        /// <exception cref=""ArgumentException"">WHY~!</exception>
        static internal void SomeWork(string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                throw new ArgumentException(""message"");
            }
            Console.WriteLine(message);
        }
    }
}
";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("ExceptionDocumentationMissingUnitTests")]
        public void MethodIncorrectTag()
        {
            var test = @"
using System;

namespace SomeTests
{
	public class BasicClass
	{
        /// <summary>
        /// asdasdasd
        /// </summary>
        /// <param name=""message""></param>
        /// <exception cref=""InvalidOperationException"">WHOOPS</exception>
        public void SomeWork(string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                throw new ArgumentException(""message"");
            }
            Console.WriteLine(message);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = ExceptionDocumentationMissingId,
                Message = String.Format(ExceptionDocumentationMissingMessageFormat, "ArgumentException"),
                Severity = DiagnosticSeverity.Error,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 17, 17)
                }
            };

            VerifyCSharpDiagnostic(test, expected);

        }

        [TestMethod]
        [TestCategory("ExceptionDocumentationMissingUnitTests")]
        public void MethodMultipleCorrect()
        {
            var test = @"
using System;

namespace SomeTests
{
	public class BasicClass
	{
        /// <summary>
        /// asdasdasd
        /// </summary>
        /// <param name=""message""></param>
        /// <exception cref=""ArgumentException"">WHY~!</exception>
        /// <exception cref=""InvalidOperationException"">WHY~!</exception>
        public void SomeWork(string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                throw new ArgumentException(""message"");
            }
            if (message[0] == 'P')
            {
                throw new InvalidOperationException(""message"");
            }
            Console.WriteLine(message);
        }
    }
}
";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("ExceptionDocumentationMissingUnitTests")]
        public void MethodMultipleNotDocumented()
        {
            var test = @"
using System;

namespace SomeTests
{
	public class BasicClass
	{
        /// <summary>
        /// asdasdasd
        /// </summary>
        /// <param name=""message""></param>
        public void SomeWork(string message)
        {
        {
            if (String.IsNullOrEmpty(message))
            {
                throw new ArgumentException(""message"");
            }
            if (message[0] == 'P')
            {
                throw new InvalidOperationException(""message"");
            }
            Console.WriteLine(message);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = ExceptionDocumentationMissingId,
                Message = String.Format(ExceptionDocumentationMissingMessageFormat, "ArgumentException"),
                Severity = DiagnosticSeverity.Error,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 17, 17)
                }
            };

            var expected2 = new DiagnosticResult
            {
                Id = ExceptionDocumentationMissingId,
                Message = String.Format(ExceptionDocumentationMissingMessageFormat, "InvalidOperationException"),
                Severity = DiagnosticSeverity.Error,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 21, 17)
                }
            };

            VerifyCSharpDiagnostic(test, expected, expected2);
        }

        [TestMethod]
        [TestCategory("ExceptionDocumentationMissingUnitTests")]
        public void OperatorCorrect()
        {
            var test = @"
using System;

namespace SomeTests
{
	public class BasicClass
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name=""x""></param>
        /// <param name=""y""></param>
        /// <returns></returns>
        /// <exception cref=""ArgumentException"">WHY~!</exception>
        static public Int32 operator +(BasicClass x, Int32 y)
        {
            if (x == null)
            {
                throw new ArgumentException(nameof(y));
            }
            return x.i + y;
        }
    }
}
";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("ExceptionDocumentationMissingUnitTests")]
        public void OperatorBad()
        {
            var test = @"
using System;

namespace SomeTests
{
	public class BasicClass
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name=""x""></param>
        /// <param name=""y""></param>
        /// <returns></returns>
        static public Int32 operator +(BasicClass x, Int32 y)
        {
            if (x == null)
            {
                throw new ArgumentException(nameof(y));
            }
            return x.i + y;
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = ExceptionDocumentationMissingId,
                Message = String.Format(ExceptionDocumentationMissingMessageFormat, "ArgumentException"),
                Severity = DiagnosticSeverity.Error,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 18, 17)
                }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        [TestCategory("ExceptionDocumentationMissingUnitTests")]
        public void PropertyCorrect()
        {
            var test = @"
using System;

namespace SomeTests
{
	public class BasicClass
	{
        private int i;

        /// <summary>
        /// MyProp
        /// </summary>
        /// <exception cref=""ArgumentOutOfRangeException"">why</exception>
        public Int32 MyProp
        {
            set
            {
                if (value < 5)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                i = value;
            }
        }
    }
}
";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("ExceptionDocumentationMissingUnitTests")]
        public void PropertyBad()
        {
            var test = @"
using System;

namespace SomeTests
{
	public class BasicClass
	{
        private int i;

        /// <summary>
        /// MyProp
        /// </summary>
        public Int32 MyProp
        {
            set
            {
                if (value < 5)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                i = value;
            }
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = ExceptionDocumentationMissingId,
                Message = String.Format(ExceptionDocumentationMissingMessageFormat, "ArgumentOutOfRangeException"),
                Severity = DiagnosticSeverity.Error,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 19, 21)
                }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        [TestCategory("ExceptionDocumentationMissingUnitTests")]
        public void ConstructorCorrect()
        {
            var test = @"
using System;

namespace SomeTests
{
	public class BasicClass
	{
        private int i;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <exception cref=""InvalidOperationException"">Why</exception>
        public BasicClass()
        {
            if (DateTimeOffset.Now.Day == 1)
            {
                throw new InvalidOperationException(nameof(DateTimeOffset.Now.Day));
            }
        }
    }
}
";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("ExceptionDocumentationMissingUnitTests")]
        public void ConstructorBad()
        {
            var test = @"
using System;

namespace SomeTests
{
	public class BasicClass
	{
        private int i;

        /// <summary>
        /// The constructor.
        /// </summary>
        public BasicClass()
        {
            if (DateTimeOffset.Now.Day == 1)
            {
                throw new InvalidOperationException(nameof(DateTimeOffset.Now.Day));
            }
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = ExceptionDocumentationMissingId,
                Message = String.Format(ExceptionDocumentationMissingMessageFormat, "InvalidOperationException"),
                Severity = DiagnosticSeverity.Error,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 17, 17)
                }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        [TestCategory("ExceptionDocumentationMissingUnitTests")]
        public void ConversionOperatorCorrect()
        {
            var test = @"
using System;

namespace SomeTests
{
	public class BasicClass
	{
        private int i;

        public BasicClass(Int32 v)
        {
            this.i = v;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""v""></param>
        /// <exception cref=""ArgumentException"">Why</exception>
        public static explicit operator BasicClass(Int32 v)
        {
            if (v < 5)
            {
                throw new ArgumentException(nameof(v));
            }
            BasicClass d = new BasicClass(v);
            return d;
        }
    }
}
";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("ExceptionDocumentationMissingUnitTests")]
        public void ConversionOperatorBad()
        {
            var test = @"
using System;

namespace SomeTests
{
	public class BasicClass
	{
        private int i;

        public BasicClass(Int32 v)
        {
            this.i = v;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""v""></param>
        public static explicit operator BasicClass(Int32 v)
        {
            if (v < 5)
            {
                throw new ArgumentException(nameof(v));
            }
            BasicClass d = new BasicClass(v);
            return d;
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = ExceptionDocumentationMissingId,
                Message = String.Format(ExceptionDocumentationMissingMessageFormat, "ArgumentException"),
                Severity = DiagnosticSeverity.Error,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 23, 17)
                }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        [TestCategory("ExceptionDocumentationMissingUnitTests")]
        public void IndexOperatorCorrect()
        {
            var test = @"
using System;

namespace SomeTests
{
	public class BasicClass
	{
        private int i;

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""key""></param>
        /// <returns></returns>
        /// <exception cref=""ArgumentOutOfRangeException"">WHY</exception>
        public int this[int key]
        {
            get
            {
                if (i < 5)
                {
                    throw new ArgumentOutOfRangeException(nameof(key));
                }
                return i;
            }
            set
            {
                i = value;
            }
        }
    }
";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("ExceptionDocumentationMissingUnitTests")]
        public void IndexOperatorMultipleCorrect()
        {
            var test = @"
using System;

namespace SomeTests
{
	public class BasicClass
	{
        private int i;

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""key""></param>
        /// <returns></returns>
        /// <exception cref=""ArgumentOutOfRangeException"">WHY</exception>
        public int this[int key]
        {
            get
            {
                if (i < 5)
                {
                    throw new ArgumentOutOfRangeException(nameof(key));
                }
                return i;
            }
            set
            {
                if (value < 5)
                {
                    throw new ArgumentOutOfRangeException(nameof(key));
                }
                i = value;
            }
        }
    }
";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("ExceptionDocumentationMissingUnitTests")]
        public void IndexOperatorMultipleDifferentCorrect()
        {
            var test = @"
using System;

namespace SomeTests
{
	public class BasicClass
	{
        private int i;

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""key""></param>
        /// <returns></returns>
        /// <exception cref=""ArgumentOutOfRangeException"">WHY</exception>
        /// <exception cref=""ArgumentException"">WHY</exception>
        public int this[int key]
        {
            get
            {
                if (i < 5)
                {
                    throw new ArgumentOutOfRangeException(nameof(key));
                }
                return i;
            }
            set
            {
                if (value < 5)
                {
                    throw new ArgumentException(nameof(key));
                }
                i = value;
            }
        }
    }
";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("ExceptionDocumentationMissingUnitTests")]
        public void IndexOperatorBad()
        {
            var test = @"
using System;

namespace SomeTests
{
	public class BasicClass
	{
        private int i;

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""key""></param>
        /// <returns></returns>
        public int this[int key]
        {
            get
            {
                if (i < 5)
                {
                    throw new ArgumentOutOfRangeException(nameof(key));
                }
                return i;
            }
            set
            {
                i = value;
            }
        }
    }
";
            var expected = new DiagnosticResult
            {
                Id = ExceptionDocumentationMissingId,
                Message = String.Format(ExceptionDocumentationMissingMessageFormat, "ArgumentOutOfRangeException"),
                Severity = DiagnosticSeverity.Error,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 21, 21)
                }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        [TestCategory("ExceptionDocumentationMissingUnitTests")]
        public void MethodCorrectFullName()
        {
            var test = @"
using System;

namespace SomeTests
{
	public class BasicClass
	{
        /// <summary>
        /// asdasdasd
        /// </summary>
        /// <param name=""message""></param>
        /// <exception cref=""System.ArgumentException"">WHY~!</exception>
        public void SomeWork(string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                throw new ArgumentException(""message"");
            }
            Console.WriteLine(message);
        }
    }
}
";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("ExceptionDocumentationMissingUnitTests")]
        public void MethodHasExceptionButNoText()
        {
            var test = @"
using System;

namespace SomeTests
{
	public class BasicClass
	{
        /// <summary>
        /// asdasdasd
        /// </summary>
        /// <param name=""message""></param>
        /// <exception cref=""ArgumentException""></exception>        
        public void SomeWork(string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                throw new ArgumentException(""message"");
            }
            Console.WriteLine(message);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = ExceptionDocumentationMissingId,
                Message = String.Format(ExceptionDocumentationMissingMessageFormat, "ArgumentException"),
                Severity = DiagnosticSeverity.Error,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 17, 17)
                }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ExceptionDocumentationMissingAnalyzer();
        }
    }
}
