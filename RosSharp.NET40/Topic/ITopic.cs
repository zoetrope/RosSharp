namespace RosSharp.Topic
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
