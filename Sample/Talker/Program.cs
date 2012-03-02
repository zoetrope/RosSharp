using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using RosSharp;

namespace Talker
{
    class Program
    {
        static void Main(string[] args)
        {
            ROS.Initialize(new Uri("http://192.168.11.5:11311/"));

            var node = ROS.CreateNode();

            var publisher = node.CreatePublisher<RosSharp.StdMsgs.String>("chatter");

            
            foreach (var i in Enumerable.Range(0,100))
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                publisher.OnNext(new RosSharp.StdMsgs.String() { data = "test : " + i });
            }

            Console.WriteLine("Press Any Key.");
            Console.ReadKey();
        }
    }
}
