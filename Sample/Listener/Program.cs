using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RosSharp;

namespace Listener
{
    class Program
    {
        static void Main(string[] args)
        {
            ROS.Initialize(new Uri("http://192.168.11.5:11311/"), "192.168.11.2");

            var node = ROS.CreateNode("Listener");

            var subscriber = node.CreateSubscriber<RosSharp.std_msgs.String>("chatter");

            subscriber.Subscribe(x => Console.WriteLine(x.data));

            Console.ReadKey();
        }
    }
}
