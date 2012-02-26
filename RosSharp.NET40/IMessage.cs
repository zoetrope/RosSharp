using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RosSharp
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
