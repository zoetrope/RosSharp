using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using RosSharp;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ROS.Initialize();
            ROS.MasterUri = new Uri("http://192.168.11.5:11311/");
            ROS.HostName = "192.168.11.3";

            var node = ROS.CreateNode("Client");

            var proxy = node.CreateProxy<AddTwoInts>("/add_two_ints").Result;
            
            //Thread.Sleep(TimeSpan.FromSeconds(3));

            var res1 = proxy.Invoke(new AddTwoInts.Request() {a = 1, b = 2});
            Console.WriteLine(res1.sum);

            Thread.Sleep(TimeSpan.FromSeconds(3));

            var res2 = proxy.Invoke(new AddTwoInts.Request() {a = 3, b = 4});
            Console.WriteLine(res2.sum);

            Console.WriteLine("Press Any Key.");
            Console.ReadKey();
        }

    }
}
