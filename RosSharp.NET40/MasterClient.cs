using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CookComputing.XmlRpc;
using System.Reactive.Linq;

namespace RosSharp
{
    public class MasterClient
    {
        private IMaster _master;
        public MasterClient()
        {
            _master = XmlRpcProxyGen.Create<IMaster>();
            _master.Url = "http://192.168.11.4:11311/";
        }

        public List<Uri> RegisterSubscriber()
        {
            //return _master.RegisterSubscriber();

            //Task<List<Uri>> task = Task.Factory.StartNew(() => new List<Uri>());

            //task.ToObservable();

            throw new NotImplementedException();
        }

        /// <summary>
        /// Looking Uri about publishers and subscribers
        /// </summary>
        /// <param name="callerId">ROS Caller ID</param>
        /// <param name="nodeName">Name of node to lookup</param>
        /// <returns>Uri of the node with associated nodeName/callerId</returns>
        public IObservable<Uri> LookupNodeAsync(string callerId, string nodeName)
        {
            return Observable.Start(()=> _master.LookupNode(callerId, nodeName))
                .Select(ret =>
                {
                    if ((int)ret[0] == 1)
                    {
                        return new Uri((string)ret[2]);
                    }
                    else
                    {
                        throw new InvalidOperationException((string)ret[1]);
                    }
                });

        }

        /// <summary>
        /// Get system state
        /// </summary>
        /// <param name="callerId">ROS Caller ID</param>
        /// <returns>system state (publishers, subscribers, services)</returns>
        public IObservable<SystemState> GetSystemStateAsync(string callerId)
        {
            return Observable.Start(() => _master.GetSystemState(callerId))
                .Select(ret =>
                {
                    if ((int)ret[0] == 1)
                    {
                        return new SystemState()
                                   {
                                       Publishers = ((object[][][])ret[2])[0]
                                           .Select(x => new PublisherSystemState()
                                                            {
                                                                TopicName = (string)x[0],
                                                                Publishers = ((object[])x[1]).Cast<string>().ToList()
                                                            }).ToList(),
                                       Subscribers = ((object[][][])ret[2])[1]
                                           .Select(x => new SubscriberSystemState()
                                                            {
                                                                TopicName = (string)x[0],
                                                                Subscribers = ((object[])x[1]).Cast<string>().ToList()
                                                            }).ToList(),
                                       Services = ((object[][][])ret[2])[2]
                                           .Select(x => new ServiceSystemState()
                                                            {
                                                                ServiceName = (string)x[0],
                                                                Services = ((object[])x[1]).Cast<string>().ToList()
                                                            }).ToList()
                                   };
                    }
                    else
                    {
                        throw new InvalidOperationException((string)ret[1]);
                    }
                });
        }
        
    }

    public class SystemState
    {
        public List<PublisherSystemState> Publishers { get; set; }
        public List<SubscriberSystemState> Subscribers { get; set; }
        public List<ServiceSystemState> Services { get; set; }
    }

    public class PublisherSystemState
    {
        public string TopicName { get; set; }
        public List<string> Publishers { get; set; }
    }
    public class SubscriberSystemState
    {
        public string TopicName { get; set; }
        public List<string> Subscribers { get; set; }
    }
    public class ServiceSystemState
    {
        public string ServiceName { get; set; }
        public List<string> Services { get; set; }
    }
}
