using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosSharp.Topic
{
    internal interface ISubscriber : ITopic
    {
        void UpdatePublishers(List<Uri> publishers);
    }
}
