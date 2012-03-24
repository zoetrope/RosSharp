using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RosSharp.Master;

namespace RosSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            ROS.Initialize();
            ROS.MasterUri = new Uri("http://192.168.11.5:11311/");
            ROS.HostName = "192.168.11.3";

            var masterServer = new MasterServer(11311);

            Console.ReadKey();
        }
    }
}
