using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using RosSharp;

namespace Talker
{
    class Program
    {
        static void Main(string[] args)
        {
            ROS.Initialize(new Uri("http://192.168.11.4:11311/"), "192.168.11.3");

            var node = ROS.CreateNode();

            var publisher = node.CreatePublisher<RosSharp.StdMsgs.String>("chatter");

            //TODO: 接続する前に送信できてしまうのはまずい。CreatePublisherを非同期にすべき。
            Console.WriteLine("Press Any Key. Start Send.");
            Console.ReadKey();
            
            foreach (var i in Enumerable.Range(0,100))
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                var data = new RosSharp.StdMsgs.String() { data = "test : " + i };
                Console.WriteLine("data = {0}", data.data);
                publisher.OnNext(data);
            }

            Console.WriteLine("Press Any Key.");
            Console.ReadKey();
        }
    }
}
