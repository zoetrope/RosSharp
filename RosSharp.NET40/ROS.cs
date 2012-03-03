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
        private static Uri _masterUri;
        private static string _localHostName;

        public static void Initialize(Uri masterUri = null, string localHostName = null)
        {
            if(masterUri == null)
            {
                _masterUri = new Uri("localhost:11311");
            }
            else
            {
                _masterUri = masterUri;
            }

            if (string.IsNullOrEmpty(localHostName))
            {
                _localHostName = Dns.GetHostName();
                //var ipaddresses = Observable.FromAsyncPattern<string, IPAddress[]>(
                //Dns.BeginGetHostAddresses, Dns.EndGetHostAddresses).Invoke(Dns.GetHostName()).First();
                //_localHostName = ipaddresses.First();
            }
            else
            {
                _localHostName = localHostName;
            }



            
        }

        public static INode CreateNode()
        {
            return new RosNode(_masterUri, _localHostName);
        }

    }
}
