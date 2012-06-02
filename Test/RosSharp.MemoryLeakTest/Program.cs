using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RosSharp.Master;

namespace RosSharp.MemoryLeakTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Ros.HostName = "localhost";
            Ros.MasterUri = new Uri("http://localhost:11311/");
            Ros.TopicTimeout = 10000;
            Ros.XmlRpcTimeout = 10000;

            var masterServer = new MasterServer(11311);


            GC.Collect();
            Console.WriteLine("First Memory = {0}", GC.GetTotalMemory(false));

            for(int i=0; i<1000; i++)
            {
                NodeTest(i);

                if (i % 100 == 0)
                {
                    GC.Collect();
                    Console.WriteLine("Total Memory = {0}", GC.GetTotalMemory(false));
                }
            }

            GC.Collect();
            Console.WriteLine("Last Memory = {0}", GC.GetTotalMemory(false));

            Console.ReadKey();
        }

        static void NodeTest(int index)
        {
            var nodeName = "test" + index;

            var node = Ros.CreateNodeAsync(nodeName, enableLogger: false).Result;

            node.Dispose();
        }
    }
}
