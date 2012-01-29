using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosSharp
{
    public class GraphName
    {
    }

    public interface ITopic
    {
        GraphName TopicName{get;}
        string TopicMessageType { get; }
    }
}
