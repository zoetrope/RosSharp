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

            var runner = new TestRunner() {TestCount = 100, PrintCount = 10};

//            runner.Run(new NodeTest());
            runner.Run(new PublisherTest());
//            runner.Run(new SubscriberTest());
//            runner.Run(new ServiceServerTest());
//            runner.Run(new ServiceProxyTest());
//            runner.Run(new ParameterTest());

            masterServer.Dispose();
            Ros.Dispose();

            Console.WriteLine("Finished All Memory Leak Test.");
            Console.WriteLine("Press Any Key.");
            Console.ReadKey();
        }

    }
}
