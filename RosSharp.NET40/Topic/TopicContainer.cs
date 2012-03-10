using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosSharp.Topic
{
    public class TopicContainer
    {


        public List<ITopic> GetPublishers()
        {
            return  new List<ITopic>();
        }

        public List<ITopic> GetSubscribers()
        {
            return new List<ITopic>();
        }
    }
}
