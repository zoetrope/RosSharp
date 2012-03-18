using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RosSharp;
using RosSharp.Message;
using RosSharp.Service;

namespace Server
{
    public class AddTwoInts : IService<AddTwoInts.Request, AddTwoInts.Response>
    {
        public string ServiceType
        {
            get { return "test_ros/AddTwoInts";}
        }

        public string Md5Sum
        {
            get { return "6a2e34150c00229791cc89ff309fff21"; }
        }

        public string ServiceDefinition
        {
            get { throw new NotImplementedException(); }
        }

        public class Request : IMessage
        {
            public string MessageType
            {
                get { return "test_ros/AddTwoIntsRequest"; }
            }

            public string Md5Sum
            {
                get { return "36d09b846be0b371c5f190354dd3153e"; }
            }

            public string MessageDefinition
            {
                get { throw new NotImplementedException(); }
            }

            public int SerializeLength
            {
                get { return 8 + 8; }
            }

            public void Serialize(BinaryWriter stream)
            {
                stream.Write(a);
                stream.Write(b);
            }

            public void Deserialize(BinaryReader stream)
            {
                a = stream.ReadInt64();
                b = stream.ReadInt64();
            }

            public long a { get; set; }
            public long b { get; set; }


            public bool Equals(Request other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return other.a == a && other.b == b;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof(Request)) return false;
                return Equals((Request)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (a.GetHashCode() * 397) ^ b.GetHashCode();
                }
            }
        }

        public class Response : IMessage
        {
            public string MessageType
            {
                get { return "test_ros/AddTwoIntsResponse"; }
            }

            public string Md5Sum
            {
                get { return "b88405221c77b1878a3cbbfff53428d7"; }
            }

            public string MessageDefinition
            {
                get { throw new NotImplementedException(); }
            }

            public int SerializeLength
            {
                get { return 8; }
            }

            public void Serialize(BinaryWriter stream)
            {
                stream.Write(c);
            }

            public void Deserialize(BinaryReader stream)
            {
                c = stream.ReadInt64();
            }
            public long c { get; set; }


            public bool Equals(Response other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return other.c == c;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof(Response)) return false;
                return Equals((Response)obj);
            }

            public override int GetHashCode()
            {
                return c.GetHashCode();
            }
        }
    }
}
