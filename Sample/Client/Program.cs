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
            ROS.Initialize(new Uri("http://192.168.11.4:11311/"), "192.168.11.3");

            var node = ROS.CreateNode();

            var proxy = node.CreateProxy<AddTwoInts, AddTwoInts.Request, AddTwoInts.Response>("/add_two_ints");

            //Thread.Sleep(TimeSpan.FromSeconds(3));

            proxy(new AddTwoInts.Request() { a = 1, b = 2 }).Subscribe(x => Console.WriteLine(x.c));

            Thread.Sleep(TimeSpan.FromSeconds(3));

            // 間を開けずに連続で呼び出すと同じ値が返ってくる。バグではないけど微妙。
            proxy(new AddTwoInts.Request() { a = 3, b = 4 }).Subscribe(x => Console.WriteLine(x.c));

            Console.WriteLine("Press Any Key.");
            Console.ReadKey();
        }

    }
}
