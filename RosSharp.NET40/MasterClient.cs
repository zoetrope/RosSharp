using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
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
            
            (_master as XmlRpcClientProtocol).Url = "http://192.168.11.4:11311/";
        }

        public SystemState GetSystemState(string callerId)
        {
            var ret = _master.GetSystemState(callerId);

            var state = new SystemState()
            {
                Code = (int)ret[0],
                StatusMessage = (string)ret[1],
                Publishers = ((object[][][])ret[2])[0]
                    .Select(x=>new PublisherSystemState(){
                        TopicName = (string)x[0],
                        Publishers = ((object[])x[1]).Cast<string>().ToList()
                    }).ToList(),
                Subscribers = ((object[][][])ret[2])[0]
                    .Select(x => new SubscriberSystemState()
                    {
                        TopicName = (string)x[0],
                        Subscribers = ((object[])x[1]).Cast<string>().ToList()
                    }).ToList(),
                Services = ((object[][][])ret[2])[0]
                    .Select(x => new ServiceSystemState()
                    {
                        ServiceName = (string)x[0],
                        Services= ((object[])x[1]).Cast<string>().ToList()
                    }).ToList(),

            };

            return state;
        }

        public List<Uri> RegisterSubscriber()
        {
            //return _master.RegisterSubscriber();

            Task<List<Uri>> task = Task.Factory.StartNew(() => new List<Uri>());

            task.ToObservable();

            throw new NotImplementedException();
        }
    }

    public class SystemState
    {
        public int Code { get; set; }
        public string StatusMessage { get; set; }

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
