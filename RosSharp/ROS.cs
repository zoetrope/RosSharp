using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using Common.Logging;
using Common.Logging.Simple;
using RosSharp.Node;

namespace RosSharp
{
    /// <summary>
    /// 
    /// </summary>
    public static class ROS
    {
        public static Uri MasterUri { get; set; }
        public static string HostName { get; set; }
        public static int XmlRpcTimeout { get; set; }
        public static int TopicTimeout { get; set; }

        public static void Initialize()
        {
            MasterUri = ReadMasterUri();
            HostName = ReadHostName();
            XmlRpcTimeout = ReadXmlRpcTimeout();
            TopicTimeout = ReadTopicTimeout();

            if (LogManager.Adapter is NoOpLoggerFactoryAdapter) //if not setting logger, logger is NoOpLogger
            {
                var nv = new NameValueCollection();
                nv["level"] = "DEBUG";
                nv["showLogName"] = "true";
                nv["showDataTime"] = "true";
                nv["dateTimeFormat"] = "yyyy/MM/dd HH:mm:ss:fff";
                LogManager.Adapter = new RosOutLoggerFactoryAdapter(nv);
            }

            Console.CancelKeyPress += (sender, args) => Dispose();
        }

        public static void Dispose()
        {
            lock (_nodes)
            {
                _nodes.Values.ToList().ForEach(node => node.Dispose());
                _nodes.Clear();
            }
        }

        private static Uri ReadMasterUri()
        {
            var variable = Environment.GetEnvironmentVariable("ROS_MASTER_URI");
            if (variable != null)
            {
                try
                {
                    return new Uri(variable);
                }
                catch(UriFormatException)
                {
                }
            }

            if(ConfigurationSection.Instance != null)
            {
                var conf = ConfigurationSection.Instance.MasterUri.Value;
                try
                {
                    return new Uri(conf);
                }
                catch (UriFormatException)
                {
                }
            }

            return new Uri("http://localhost:11311");
        }


        private static string ReadHostName()
        {
            var variable = Environment.GetEnvironmentVariable("ROS_HOSTNAME");
            if (!string.IsNullOrEmpty(variable))
            {
                return variable;
            }

            if (ConfigurationSection.Instance != null)
            {
                var conf = ConfigurationSection.Instance.HostName.Value;
                if (!string.IsNullOrEmpty(conf))
                {
                    return conf;
                }
            }

            return Dns.GetHostName();
        }


        private static int ReadXmlRpcTimeout()
        {
            var variable = Environment.GetEnvironmentVariable("ROS_XMLRPC_TIMEOUT");
            if (!string.IsNullOrEmpty(variable))
            {
                int timeout;
                if(int.TryParse(variable, out timeout))
                {
                    return timeout;
                }
            }

            if (ConfigurationSection.Instance != null)
            {
                return ConfigurationSection.Instance.XmlRpcTimeout.Value;
            }

            return 1000;
        }
        private static int ReadTopicTimeout()
        {
            var variable = Environment.GetEnvironmentVariable("ROS_TOPIC_TIMEOUT");
            if (!string.IsNullOrEmpty(variable))
            {
                int timeout;
                if (int.TryParse(variable, out timeout))
                {
                    return timeout;
                }
            }

            if (ConfigurationSection.Instance != null)
            {
                return ConfigurationSection.Instance.TopicTimeout.Value;
            }

            return 1000;
        }

        private static readonly Dictionary<string, INode> _nodes = new Dictionary<string, INode>();
        public static INode CreateNode(string nodeName)
        {
            lock (_nodes)
            {
                var node = new RosNode(nodeName);
                _nodes.Add(nodeName, node);
                return node;
            }
        }

    }
}
