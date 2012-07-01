//----------------------------------------------------------------
// <auto-generated>
//     This code was generated by the GenMsg. Version: 0.1.0.0
//     Don't change it manually.
//     2012-06-23T22:05:30+09:00
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
    public class Char : IMessage
    {
        ///<exclude/>
        public Char()
        {
        }
        ///<exclude/>
        public Char(BinaryReader br)
        {
            Deserialize(br);
        }
        ///<exclude/>
        public sbyte data { get; set; }
        ///<exclude/>
        public string MessageType
        {
            get { return "std_msgs/Char"; }
        }
        ///<exclude/>
        public string Md5Sum
        {
            get { return "1bf77f25acecdedba0e224b162199717"; }
        }
        ///<exclude/>
        public string MessageDefinition
        {
            get { return "char data"; }
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
            data = br.ReadSByte();
        }
        ///<exclude/>
        public int SerializeLength
        {
            get { return 1; }
        }
        ///<exclude/>
        public bool Equals(Char other)
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
            if (obj.GetType() != typeof(Char)) return false;
            return Equals((Char)obj);
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