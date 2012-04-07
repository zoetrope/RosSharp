using System;

namespace RosSharp.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            ROS.Initialize();
            ROS.MasterUri = new Uri("http://192.168.11.5:11311/");
            ROS.HostName = "192.168.11.3";

            var node = ROS.CreateNode("Server");

            var param = node.CreateParameterAsync<string>("rosversion").Result;

            param.Subscribe(x => Console.WriteLine(x));

            param.Value = "test";
            
            Console.WriteLine("Press Any Key.");
            Console.ReadKey();
        }
    }
}
