using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using RosSharp;

namespace Talker
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MasterClient(new Uri("http://192.168.11.4:11311/"));

            //var state = client.GetSystemStateAsync("/talker").First();
            
            //var uri = client.LookupNodeAsync("/talker", "/rosout").First();

            //var uri = client.GetUriAsync("/talker").First();
            //var uri = client.GetUriAsync(null);


            //var ret = client.RegisterServiceAsync("/talker", "myservice", new Uri("http://192.168.11.2:11112"), new Uri("http://192.168.11.2:11111")).First();
            //client.UnregisterServiceAsync("/talker", null, new Uri("http://localhost")).First();
            
            //var ret1 = client.RegisterSubscriberAsync("/test", "topic1", "std_msgs/String", new Uri("http://192.168.11.2:11112")).First();
            //var ret2 = client.RegisterPublisherAsync("/test", "topic1", "std_msgs/String", new Uri("http://192.168.11.2:11112")).First();
            var ret2 = client.RegisterPublisherAsync("/test", "chatter", "std_msgs/String", new Uri("http://192.168.11.2:11112")).First();
        }
    }
}
