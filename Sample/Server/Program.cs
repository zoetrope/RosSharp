using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using RosSharp;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ROS.Initialize(new Uri("http://192.168.11.5:11311/"));

            var node = ROS.CreateNode();

            node.RegisterService<AddTwoInts, AddTwoInts.Request, AddTwoInts.Response>("/add_two_ints", add_two_ints);


            Console.WriteLine("Press Any Key.");
            Console.ReadKey();
        }

        static AddTwoInts.Response add_two_ints(AddTwoInts.Request req)
        {
            Console.WriteLine("add_two_ints: a = {0}, b = {1}", req.a, req.b);
            return new AddTwoInts.Response() { c = req.a + req.b };
        }

    }


}
