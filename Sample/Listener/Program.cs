using System;
using System.Reactive.Linq;
using System.Threading;

namespace RosSharp.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Ros.MasterUri = new Uri("http://192.168.11.4:11311/");
            Ros.HostName = "192.168.11.3";

            var node = Ros.CreateNodeAsync("/Listener").Result;

            var subscriber = node.CreateSubscriberAsync<RosSharp.std_msgs.String>("/chatter").Result;

            subscriber.WaitForConnection();

            subscriber.Subscribe(x => Console.WriteLine(x.data));

            Thread.Sleep(Timeout.Infinite);
            Ros.Dispose();
        }
    }
}
