using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RosSharp.Message;

namespace RosSharp.StdMsgs
{
    public class String : IMessage
    {
        public string MessageType
        {
            get { return "std_msgs/String"; }
        }

        public string Md5Sum
        {
            get { return "992ce8a1687cec8c8bd883ec73ca41d1"; }
        }

        public string MessageDefinition
        {
            get { return "string data"; }
        }

        public int SerializeLength
        {
            get { return 4 + data.Length; }
        }

        public void Serialize(BinaryWriter stream)
        {
            stream.WriteUtf8String(data);
        }

        public void Deserialize(BinaryReader stream)
        {
            data = stream.ReadUtf8String();
        }

        public string data { get; set; }

        public bool Equals(String other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.data, data);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (String)) return false;
            return Equals((String) obj);
        }

        public override int GetHashCode()
        {
            return (data != null ? data.GetHashCode() : 0);
        }
    }
}
