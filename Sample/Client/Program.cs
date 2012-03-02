using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using RosSharp;
using Server;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ROS.Initialize();

            var node = ROS.CreateNode();

            var proxy = node.CreateProxy<AddTwoInts, AddTwoIntsReq, AddTwoIntsRes>("/add_two_ints");

            //Thread.Sleep(TimeSpan.FromSeconds(3));

            proxy(new AddTwoIntsReq() { a = 1, b = 2 });

            Console.WriteLine("Press Any Key.");
            Console.ReadKey();
        }

    }
}
