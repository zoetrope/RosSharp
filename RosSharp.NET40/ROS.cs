using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using RosSharp.Node;

namespace RosSharp
{
    public class ROS
    {
        //TODO: staticではなく、インスタンスを作るべきか？
        public static Uri MasterUri { get; private set; }
        public static string LocalHostName { get; private set; }

        public static void Initialize(Uri masterUri = null, string localHostName = null)
        {
            if(masterUri == null)
            {
                MasterUri = new Uri("localhost:11311");
            }
            else
            {
                MasterUri = masterUri;
            }

            if (string.IsNullOrEmpty(localHostName))
            {
                LocalHostName = Dns.GetHostName();
                //var ipaddresses = Observable.FromAsyncPattern<string, IPAddress[]>(
                //Dns.BeginGetHostAddresses, Dns.EndGetHostAddresses).Invoke(Dns.GetHostName()).First();
                //LocalHostName = ipaddresses.First();
            }
            else
            {
                LocalHostName = localHostName;
            }



            
        }

        public static INode CreateNode(string nodeName)
        {
            return new RosNode(nodeName);
        }

    }
}
