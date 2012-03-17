using System.IO;

namespace RosSharp.Message
{
    public interface IMessage
    {
        string MessageType { get; }
        string Md5Sum { get; }
        string MessageDefinition { get; }

        int SerializeLength { get; }

        void Serialize(BinaryWriter stream);
        void Deserialize(BinaryReader stream);
    }
}
