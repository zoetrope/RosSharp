using System.IO;

namespace RosSharp.Message
{
    public interface IMessage
    {
        string MessageType { get; }
        string Md5Sum { get; }
        string MessageDefinition { get; }

        void Serialize(Stream stream);
        void Deserialize(Stream stream);
    }
}
