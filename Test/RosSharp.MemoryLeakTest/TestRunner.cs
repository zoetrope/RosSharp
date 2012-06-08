using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RosSharp.MemoryLeakTest
{
    class TestRunner
    {

        public int TestCount { get; set; }
        public int PrintCount { get; set; }

        public void Run(ITest test)
        {
            Console.WriteLine("******************* Start {0} *******************", test.GetType().Name);

            PrintMemorySize("first");

            test.Initialize();


            for (int i = 0; i < TestCount; i++)
            {
                test.Do(i);

                if (i % PrintCount == 0)
                {
                    PrintMemorySize("do(" + i + ")");
                }
            }

            test.Cleanup();

            PrintMemorySize("last");

            Console.WriteLine("******************* Finish {0} *******************", test.GetType().Name);
        }

        private void PrintMemorySize(string message)
        {
            GC.Collect();
            Console.WriteLine("Memory={0}, Message={1}", GC.GetTotalMemory(false), message);
        }

    }
}
