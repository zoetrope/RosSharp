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
            //var client = new MasterClient(new Uri("http://192.168.11.5:11311/"));

            //var state = client.GetSystemStateAsync("/talker").First();
            
            //var uri = client.LookupNodeAsync("/talker", "/rosout").First();

            //var uri = client.GetUriAsync("/talker").First();
            //var uri = client.GetUriAsync(null);


            //var ret = client.RegisterServiceAsync("/talker", "myservice", new Uri("http://192.168.11.2:11112"), new Uri("http://192.168.11.2:11111")).First();
            //client.UnregisterServiceAsync("/talker", null, new Uri("http://localhost")).First();

            //var ret1 = client.RegisterSubscriberAsync("/test", "chatter", "std_msgs/String", new Uri("http://192.168.11.2:11112")).First();
            //var ret2 = client.RegisterPublisherAsync("/test", "topic1", "std_msgs/String", new Uri("http://192.168.11.2:11112")).First();
            //var ret2 = client.RegisterPublisherAsync("/test", "chatter", "std_msgs/String", new Uri("http://192.168.11.2:11112")).First();

            //var slave = new SlaveClient(ret1.First());

            //var pid = slave.GetPidAsync("/test").First();
            //var businfo = slave.GetBusInfoAsync("/test").First();

            //var busstate = slave.GetBusStatsAsync("/test").First(); // 失敗する？

            //var masterUir = slave.GetMasterUriAsync("/test").First();

            //var pubs = slave.GetPublicationsAsync("/test").First();
            //var topicParam = slave.RequestTopicAsync("/test", "/chatter", new object[1] { new string[1] { "TCPROS" } }).First();


            ROS.Initialize();

            var node = ROS.CreateNode();

            var publisher = node.CreatePublisher<RosSharp.StdMsgs.String>("chatter");

            
            foreach (var i in Enumerable.Range(0,100))
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                publisher.OnNext(new RosSharp.StdMsgs.String() { data = "test : " + i });
            }

            Console.WriteLine("Press Any Key.");
            Console.ReadKey();
        }
    }
}
