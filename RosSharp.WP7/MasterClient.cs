using System;
using System.Net;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace RosSharp
{
    public class MasterClient
    {
        private MasterProxy _proxy;
        public MasterClient()
        {
            _proxy = new MasterProxy();
            //_proxy.Url = 
        }

        /// <summary>
        /// Looking Uri about publishers and subscribers
        /// </summary>
        /// <param name="callerId">ROS Caller ID</param>
        /// <param name="nodeName">Name of node to lookup</param>
        /// <returns>Uri of the node with associated nodeName/callerId</returns>
        public IObservable<Uri> LookupNodeAsync(string callerId, string nodeName)
        {
            return Observable.FromAsyncPattern<string, string, object[]>(_proxy.BeginLookupNode, _proxy.EndLookupNode)
                .Invoke(callerId, nodeName)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => new Uri((string)ret[2]));
        }

    }
}
