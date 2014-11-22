#region License Terms

// ================================================================================
// RosSharp
// 
// Software License Agreement (BSD License)
// 
// Copyright (C) 2012 zoetrope
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// ================================================================================

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common.Logging;
using Common.Logging.Simple;
using Common.Logging.Configuration;

namespace RosSharp
{
    /// <summary>
    ///   ROS Manager
    /// </summary>
    public static class Ros
    {
        private static readonly Dictionary<string, Node> _nodes = new Dictionary<string, Node>();

        /// <summary>
        ///   XML-RPC URI of the Master
        /// </summary>
        /// <example>
        ///   http://192.168.1.10:11311/
        /// </example>
        public static Uri MasterUri { get; set; }

        /// <summary>
        ///   local network address of a ROS Node
        /// </summary>
        /// <example>
        ///   192.168.1.10
        /// </example>
        public static string HostName { get; set; }

        /// <summary>
        ///   Timeout in milliseconds on a XML-RPC proxy method call
        /// </summary>
        public static int XmlRpcTimeout { get; set; }

        /// <summary>
        ///   Timeout in milliseconds on a ROS TOPIC
        /// </summary>
        public static int TopicTimeout { get; set; }

        /// <summary>
        ///   Initialize Setting
        /// </summary>
        static Ros()
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

        /// <summary>
        ///   Dispose all nodes
        /// </summary>
        public static void Dispose()
        {
            DisposeAsync().Wait();
        }

        /// <summary>
        ///   Asynchronous dispose all nodes
        /// </summary>
        public static Task DisposeAsync()
        {
            var nodes = GetNodes();
            var tasks = nodes.Select(node => node.DisposeAsync());
            
            return Task.Factory.StartNew(() => Task.WaitAll(tasks.ToArray()));
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
                catch (UriFormatException)
                {
                }
            }

            if (ConfigurationSection.Instance != null)
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
                if (int.TryParse(variable, out timeout))
                {
                    return timeout;
                }
            }

            if (ConfigurationSection.Instance != null)
            {
                return ConfigurationSection.Instance.XmlRpcTimeout.Value;
            }

            return 5000;
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

            return 5000;
        }

        /// <summary>
        ///   Create a ROS Node
        /// </summary>
        /// <param name="nodeName"> ROS Node name </param>
        /// <param name="enableLogger"> if true, enable RosOut Logger </param>
        /// <param name="anonymous"> if true, named to an anonymous name (append a random number to the node name) </param>
        /// <returns> created Node </returns>
        public static Task<Node> InitNodeAsync(string nodeName, bool enableLogger = true, bool anonymous = false)
        {
            lock (_nodes)
            {
                var node = new Node(nodeName);

                var tcs = new TaskCompletionSource<Node>();

                var initTask = node.InitializeAsync(enableLogger);

                initTask.ContinueWith(t =>
                {
                    node.Disposing += DisposeNode;
                    _nodes.Add(nodeName, node);
                    tcs.SetResult(node);
                }, TaskContinuationOptions.OnlyOnRanToCompletion);

                initTask.ContinueWith(
                    t => tcs.SetException(t.Exception.InnerException),
                    TaskContinuationOptions.OnlyOnFaulted);

                return tcs.Task;
            }
        }

        /// <summary>
        ///   Get all nodes
        /// </summary>
        /// <returns> all nodes </returns>
        public static List<Node> GetNodes()
        {
            List<Node> nodes;
            lock (_nodes)
            {
                nodes = new List<Node>(_nodes.Values);
            }
            return nodes;
        }

        private static Task DisposeNode(string nodeId)
        {
            return Task.Factory.StartNew(() =>
            {
                lock (_nodes)
                {
                    if (_nodes.ContainsKey(nodeId))
                    {
                        _nodes.Remove(nodeId);
                    }
                }
            });
        }
    }
}