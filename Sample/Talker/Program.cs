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
            var client = new MasterClient();

            //var state = client.GetSystemStateAsync("/talker").First();
            
            //var uri = client.LookupNodeAsync("/talker", "/rosout").First();

            var uri = client.GetUriAsync("/talker").First();
            //var uri = client.GetUriAsync(null);
        }
    }
}
