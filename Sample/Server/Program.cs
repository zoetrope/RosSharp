using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using RosSharp;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ROS.Initialize();

            var node = ROS.CreateNode();

            //var publisher = node.CreateServer("chatter", add_two_ints);


            Console.WriteLine("Press Any Key.");
            Console.ReadKey();
        }

        int add_two_ints(int a, int b)
        {
            return a + b;
        }


        static void Test()
        {

            var _masterClient = new MasterClient(new Uri("http://192.168.11.5:11311/"));


            var ret1 = _masterClient
                .RegisterServiceAsync("/test", "chatter",  new Uri("http://192.168.11.2:11112"), new Uri("http://192.168.11.2:11112"))
                .First();//TODO: エラーが起きたとき

            //var slave = new SlaveClient(ret1.First());

            //var topicParam = slave.RequestTopicAsync("/test", "/chatter", new object[1] { new string[1] { "TCPROS" } }).First();

            //var subscriber = new Subscriber<TDataType>(topicParam);
        }
    }


}
