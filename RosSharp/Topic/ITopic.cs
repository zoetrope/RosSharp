namespace RosSharp.Topic
{
    public interface ITopic
    {
        string TopicName { get; }
        string MessageType { get; }
    }
}
