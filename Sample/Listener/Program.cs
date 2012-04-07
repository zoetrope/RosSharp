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
            ROS.MasterUri = new Uri("http://192.168.11.5:11311/");
            ROS.HostName = "192.168.11.3";

            var node = ROS.CreateNode("Listener");

            var subscriber = node.CreateSubscriberAsync<RosSharp.std_msgs.String>("/chatter").Result;

            subscriber.Subscribe(x => Console.WriteLine(x.data));

            Console.WriteLine("Press Any Key.");
            Console.ReadKey();
        }
    }
}
