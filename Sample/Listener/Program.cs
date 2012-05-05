using System;
using System.Threading;

namespace RosSharp.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Ros.MasterUri = new Uri("http://192.168.11.4:11311/");
            Ros.HostName = "192.168.11.2";


            var node = Ros.CreateNode("/Listener");

            var subscriber = node.CreateSubscriberAsync<RosSharp.std_msgs.String>("/chatter").Result;

            subscriber.Subscribe(
                x => Console.WriteLine(x.data),
                () => Console.WriteLine("OnCompleted!!"));

            //Console.WriteLine("Press Any Key.");
            //Console.ReadKey();

            while (true)
            {
                Thread.Sleep(TimeSpan.FromSeconds(10));
            }

            Ros.Dispose();
        }
    }
}
