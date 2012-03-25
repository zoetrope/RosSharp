using System;
using System.IO;
using System.Linq;
using RosSharp.Message;
using System.Collections.Generic;
namespace RosSharp.std_msgs
{
    public class String : IMessage
    {
        public String()
        {
            data = string.Empty;
        }
        public String(BinaryReader br)
        {
            Deserialize(br);
        }
        public string data { get; set; }
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
            get { return @"string data"; }
        }
        public void Serialize(BinaryWriter bw)
        {
            bw.WriteUtf8String(data);
        }
        public void Deserialize(BinaryReader br)
        {
            data = br.ReadUtf8String();
        }
        public int SerializeLength
        {
            get { return 4 + data.Length; }
        }
        public bool Equals(String other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.data.Equals(data);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(String)) return false;
            return Equals((String)obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int result = 0;
                result = (result * 397) ^ data.GetHashCode();
                return result;
            }
        }
    }
}