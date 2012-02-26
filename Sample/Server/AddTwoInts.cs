using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RosSharp;

namespace Server
{
    delegate AddTwoIntsRes AddTwoIntsProxy(AddTwoIntsReq req);

    public class AddTwoInts : IService<AddTwoIntsReq, AddTwoIntsRes>
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

        public AddTwoIntsRes CreateResponse()
        {
            throw new NotImplementedException();
        }

        public AddTwoIntsReq CreateRequest()
        {
            throw new NotImplementedException();
        }
    }



    public class AddTwoIntsReq : IMessage
    {
        public string MessageType
        {
            get { throw new NotImplementedException(); }
        }

        public string Md5Sum
        {
            get { throw new NotImplementedException(); }
        }

        public string MessageDefinition
        {
            get { throw new NotImplementedException(); }
        }

        public void Serialize(Stream stream)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(Stream stream)
        {
            throw new NotImplementedException();
        }
    }

    public class AddTwoIntsRes : IMessage
    {
        public string MessageType
        {
            get { throw new NotImplementedException(); }
        }

        public string Md5Sum
        {
            get { throw new NotImplementedException(); }
        }

        public string MessageDefinition
        {
            get { throw new NotImplementedException(); }
        }

        public void Serialize(Stream stream)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
