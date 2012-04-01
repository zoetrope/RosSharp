using System;
using NDesk.Options;
using RosSharp.Master;

namespace RosSharp.RosCore
{
    class Program
    {
        static void Main(string[] args)
        {
            ROS.Initialize();

            int portNumber = 11311;

            var optionSet = new OptionSet()
            {
                {"p|port=", v => portNumber = int.Parse(v)}
            };

            optionSet.Parse(args);
            
            var masterServer = new MasterServer(portNumber);

            Console.WriteLine("ROS_MASTER_URI={0}", masterServer.MasterUri);
            Console.ReadKey();
        }
    }
}
