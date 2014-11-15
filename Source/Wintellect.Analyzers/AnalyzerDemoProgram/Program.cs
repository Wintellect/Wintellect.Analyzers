using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzerDemoProgram
{
    class BasicClass
    {
        private object o;
        private int i;

        public void SomeWork(string message)
        {
            i += 1;
            lock (o)
            {
                Console.WriteLine(message);
            }
        }

        public String DoSomeParamArrays(String message1, string message2, String message3)
        {
            return String.Format("{0}{1}{2}", message1, message2, message3, message3);
        }

    }

    class Program
    {
        object oObject;
        string sString;
        Object oObject2;

        static void Main(string[] args)
        {
            String bob = args[0];
            Debug.Assert(false);
            Debug.Assert(DateTime.Now > new DateTime(1));
            Debug.Assert(DateTime.Now > new DateTime(1), "DateTime.Now > new DateTime(1)");

            if (DateTime.Now > new DateTime(1)) Console.WriteLine("It's time");

            if (DateTime.Now > new DateTime(1))
                Console.WriteLine("It's time");

            if (DateTime.Now > new DateTime(1))
                Console.WriteLine("It's time");
            else
                Console.WriteLine("Nope, not yet.");

            Debug.Assert(false, "false");

        }
    }
}
