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

            public void Serialize(Stream stream)
            {
                var bw = new BinaryWriter(stream);
                bw.Write(16);//TODO:あとでけす
                bw.Write(a);
                bw.Write(b);
            }

            public void Deserialize(Stream stream)
            {
                var br = new BinaryReader(stream);
                a = br.ReadInt64();
                b = br.ReadInt64();
            }

            public long a { get; set; }
            public long b { get; set; }
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

            public void Serialize(Stream stream)
            {
                var bw = new BinaryWriter(stream);
                bw.Write(c);
            }

            public void Deserialize(Stream stream)
            {
                var br = new BinaryReader(stream);
                br.ReadInt32();//TODO:あとで消す
                c = br.ReadInt64();
            }
            public long c { get; set; }
        }
    }



}
