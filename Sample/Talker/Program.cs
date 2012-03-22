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
            ROS.Initialize(new Uri("http://192.168.11.5:11311/"), "192.168.11.3");

            var node = ROS.CreateNode("Talker");

            var publisher = node.CreatePublisher<RosSharp.std_msgs.String>("/chatter");

            Console.WriteLine("Press Any Key. Start Send.");
            Console.ReadKey();

            int i = 0;
            while (true)
            {
                var data = new RosSharp.std_msgs.String() {data = "test : " + i++};
                Console.WriteLine("data = {0}", data.data);
                publisher.OnNext(data);
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

        }
    }
}
