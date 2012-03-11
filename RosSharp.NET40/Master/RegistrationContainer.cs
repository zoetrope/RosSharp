using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosSharp.Master
{
    internal class RegistrationContainer
    {
        
        public List<Uri> RegsiterSubscriber(string topic, string topicType, string callerApi)
        {
            return new List<Uri>();
        }

        public void UnregisterSubscriber()
        {
            
        }

        public List<Uri> RegisterPublisher(string topic, string topicType, string callerApi)
        {
            return new List<Uri>();
        }

        public void UnregisterPublisher()
        {
            
        }

    }
}
