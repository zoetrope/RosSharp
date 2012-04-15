using System;
using System.Threading;

namespace RosSharp.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            //RosManager.MasterUri = new Uri("http://192.168.11.5:11311/");
            //RosManager.HostName = "192.168.11.3";

            var node = RosManager.CreateNode("Talker");

            var publisher = node.CreatePublisherAsync<RosSharp.std_msgs.String>("/chatter").Result;

            Console.WriteLine("Press Any Key. Start Send.");
            Console.ReadKey();

            int i = 0;
            while (true)
            {
                var data = new RosSharp.std_msgs.String() {data = "test : " + i++};
                Console.WriteLine("data = {0}", data.data);
                publisher.OnNext(data);
                Thread.Sleep(TimeSpan.FromSeconds(1));
                
            }

            
        }
    }
}
