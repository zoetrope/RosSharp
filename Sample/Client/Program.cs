using System;
using System.Threading;

namespace RosSharp.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Ros.MasterUri = new Uri("http://192.168.11.5:11311/");
            Ros.HostName = "192.168.11.3";

            var node = Ros.CreateNodeAsync("Client").Result;

            var proxy = node.CreateProxyAsync<AddTwoInts>("/add_two_ints").Result;
            
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
