using System;
using RosSharp.Master;

namespace RosSharp.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Ros.MasterUri = new Uri("http://192.168.11.2:11311/");
            Ros.HostName = "192.168.11.2";

            var node = Ros.CreateNodeAsync("Server").Result;

            node.RegisterServiceAsync("/add_two_ints", new AddTwoInts(add_two_ints)).Wait();

            //node.RegisterServiceAsync("/add_two_ints",new AddTwoInts(req => new AddTwoInts.Response {sum = req.a + req.b})).Wait();
            
            Console.WriteLine("Press Any Key.");
            Console.ReadKey();
        }

        static AddTwoInts.Response add_two_ints(AddTwoInts.Request req)
        {
            Console.WriteLine("add_two_ints: a = {0}, b = {1}", req.a, req.b);
            return new AddTwoInts.Response() { sum = req.a + req.b };
        }

    }


}
