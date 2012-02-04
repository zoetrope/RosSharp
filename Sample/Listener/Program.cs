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
            ROS.Initialize();

            var node = ROS.CreateNode();

            var subscriber = node.CreateSubscriber<string>("chatter");

            subscriber.Subscribe(Console.WriteLine);

            Console.ReadKey();
        }
    }
}
