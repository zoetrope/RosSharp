//----------------------------------------------------------------
// <auto-generated>
//     This code was generated by the GenMsg. Version: 0.1.0.0
//     Don't change it manually.
//     2012-06-23T22:05:34+09:00
// </auto-generated>
//----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RosSharp.Message;
using RosSharp.Service;
using RosSharp.std_msgs;
namespace RosSharp.std_msgs
{
    ///<exclude/>
    public class UInt32 : IMessage
    {
        ///<exclude/>
        public UInt32()
        {
        }
        ///<exclude/>
        public UInt32(BinaryReader br)
        {
            Deserialize(br);
        }
        ///<exclude/>
        public uint data { get; set; }
        ///<exclude/>
        public string MessageType
        {
            get { return "std_msgs/UInt32"; }
        }
        ///<exclude/>
        public string Md5Sum
        {
            get { return "304a39449588c7f8ce2df6e8001c5fce"; }
        }
        ///<exclude/>
        public string MessageDefinition
        {
            get { return "uint32 data"; }
        }
        ///<exclude/>
        public bool HasHeader
        {
            get { return false; }
        }
        ///<exclude/>
        public void Serialize(BinaryWriter bw)
        {
            bw.Write(data);
        }
        ///<exclude/>
        public void Deserialize(BinaryReader br)
        {
            data = br.ReadUInt32();
        }
        ///<exclude/>
        public int SerializeLength
        {
            get { return 4; }
        }
        ///<exclude/>
        public bool Equals(UInt32 other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.data.Equals(data);
        }
        ///<exclude/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(UInt32)) return false;
            return Equals((UInt32)obj);
        }
        ///<exclude/>
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