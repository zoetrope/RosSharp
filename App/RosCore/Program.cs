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
            ROS.Initialize(new Uri("http://192.168.11.2:11311/"), "192.168.11.2");

            var masterServer = new MasterServer(11311);

            Console.ReadKey();
        }
    }
}
