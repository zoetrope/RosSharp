using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using CookComputing.XmlRpc;
using RosSharp.Master;
using RosSharp.Parameter;
using RosSharp.Slave;
using RosSharp.Topic;

namespace RosSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            ROS.Initialize();
            ROS.MasterUri = new Uri("http://192.168.11.5:11311/");
            ROS.HostName = "192.168.11.3";

            //var server = new SlaveServer(0, new TopicContainer(), new RosTopicServer());
            var client = new ParameterServerClient(new Uri("http://192.168.11.6:11311"));

            var v = client.GetParamAsync("test", "/gains").First();
            //var x = client.GetParamNamesAsync("test").First();

            //var a = client.SetParamAsync("test", "test_param", 123).First();
            //var b = client.SubscribeParamAsync("test", server.SlaveUri, "test_param").First();
            //var c = client.SetParamAsync("test", "test_param", 456).First();

            //var masterServer = new MasterServer(11311);
            Console.ReadKey();
        }
    }
}
