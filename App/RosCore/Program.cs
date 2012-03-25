using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
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

            int portNumber = 11311;

            if(args.Length == 2)
            {
                if (args[0] != "-p" || !int.TryParse(args[1], out portNumber))
                {
                    Console.WriteLine("Usage: RosCore [-p PORT_NUMBER]");
                    Console.WriteLine("Default Port Number is 11311");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }

            var masterServer = new MasterServer(portNumber);

            Console.WriteLine("ROS_MASTER_URI={0}", masterServer.MasterUri);
            Console.ReadKey();
        }
    }
}
