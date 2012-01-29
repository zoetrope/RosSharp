using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;
using System.Reactive.Linq;

namespace RosSharp
{
    class MasterClient
    {
        private IMaster _master;
        public MasterClient()
        {
            _master = XmlRpcProxyGen.Create<IMaster>();
            (_master as XmlRpcClientProtocol).Url = "http://myhostname:11311/";
        }

        public List<Uri> RegisterSubscriber()
        {
            //return _master.RegisterSubscriber();

            Task<List<Uri>> task = Task.Factory.StartNew(() => new List<Uri>());

            task.ToObservable();

            throw new NotImplementedException();
        }
    }
}
