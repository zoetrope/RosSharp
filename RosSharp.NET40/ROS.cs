using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using RosSharp.Node;

namespace RosSharp
{
    public static class ROS
    {
        public static Uri MasterUri { get; set; }
        public static string HostName { get; set; }

        public static void Initialize()
        {
            MasterUri = ReadMasterUri();
            HostName = ReadHostName();

            // log setting
        }

        private static Uri ReadMasterUri()
        {
            if (MasterUri != null)
            {
                return MasterUri;
            }


            var variable = Environment.GetEnvironmentVariable("ROS_MASTER_URI");
            if (variable != null)
            {
                try
                {
                    MasterUri = new Uri(variable);
                    return MasterUri;
                }
                catch(UriFormatException)
                {
                }
            }

            if(ConfigurationSection.Instance != null)
            {
                var conf = ConfigurationSection.Instance.Node.MasterUri.Value;
                try
                {
                    MasterUri = new Uri(conf);
                    return MasterUri;
                }
                catch (UriFormatException)
                {
                }
            }

            MasterUri = new Uri("http://localhost:11311");
            return MasterUri;
        }


        private static string ReadHostName()
        {
            if (!string.IsNullOrEmpty(HostName))
            {
                return HostName;
            }


            var variable = Environment.GetEnvironmentVariable("ROS_HOSTNAME");
            if (!string.IsNullOrEmpty(variable))
            {
                HostName = variable;
                return HostName;
            }

            if (ConfigurationSection.Instance != null)
            {
                var conf = ConfigurationSection.Instance.Node.MasterUri.Value;
                if (!string.IsNullOrEmpty(conf))
                {
                    HostName = conf;
                    return HostName;
                }
            }

            HostName = Dns.GetHostName();
            return HostName;
        }

        public static INode CreateNode(string nodeName)
        {
            return new RosNode(nodeName);
        }

    }
}
