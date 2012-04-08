using System;

namespace RosSharp.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            RosManager.Initialize();
            RosManager.MasterUri = new Uri("http://192.168.11.5:11311/");
            RosManager.HostName = "192.168.11.3";

            var node = RosManager.CreateNode("Listener");

            var subscriber = node.CreateSubscriberAsync<RosSharp.std_msgs.String>("/chatter").Result;

            subscriber.Subscribe(x => Console.WriteLine(x.data));

            Console.WriteLine("Press Any Key.");
            Console.ReadKey();
        }
    }
}
