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

            var node = Ros.CreateNodeAsync("/Talker").Result;
            //Console.ReadKey();

            var publisher = node.CreatePublisherAsync<RosSharp.std_msgs.String>("/chatter").Result;
            
            publisher.OnConnectedAsObservable().First();

            Console.WriteLine("Start Send");

            //Console.WriteLine("Press Any Key. Start Send.");
            //Console.ReadKey();

            int i = 0;
            while (true)
            {
                var data = new RosSharp.std_msgs.String() {data = "test : " + i++};
                Console.WriteLine("data = {0}", data.data);
                publisher.OnNext(data);
                Thread.Sleep(TimeSpan.FromSeconds(1));
                
            }

            //Ros.Dispose();
        }
    }
}
