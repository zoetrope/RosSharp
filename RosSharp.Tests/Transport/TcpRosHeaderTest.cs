using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Topic;
using RosSharp.Transport;

namespace RosSharp.Tests.Transport
{
    [TestClass]
    public class TcpRosHeaderTest
    {

        [TestMethod]
        public void SerializeSubscriberHeader_Success()
        {
            var data = new std_msgs.String() { data = "test" };

            var header = new 
            {
                callerid = "test",
                topic = "/chatter",
                md5sum = data.Md5Sum,
                type = data.MessageType
            };

            var ms = new MemoryStream();

            TcpRosHeaderSerializer.Serialize(ms, header);

            var array = ms.ToArray();
            array.Is(new byte[] { 
                // length
                102,0,0,0,
                    // caller_id length
					13,0,0,0,
                        // caller_id=test
						99,97,108,108,101,114,105,100,61,116,101,115,116,
                    // topci length
					14,0,0,0,
                        // topic=/chatter
						116,111,112,105,99,61,47,99,104,97,116,116,101,114,
                    // md5sum length
					39,0,0,0,
                        // md5sum=992ce8a1687cec8c8bd883ec73ca41d1
						109,100,53,115,117,109,61,57,57,50,99,101,56,97,49,54,56,55,99,101,99,56,99,56,98,100,56,56,51,101,99,55,51,99,97,52,49,100,49,
                    // type length
					20,0,0,0,
                        // type=std_msgs/String
                        116,121,112,101,61,115,116,100,95,109,115,103,115,47,83,116,114,105,110,103
            });

            ms.Close();
        }
        [TestMethod]
        public void DesrializeSubscriberHeader_Success()
        {
            var msg = new byte[] { 
                // length
                102,0,0,0,
                    // caller_id length
					13,0,0,0,
                        // caller_id=test
						99,97,108,108,101,114,105,100,61,116,101,115,116,
                    // topci length
					14,0,0,0,
                        // topic=/chatter
						116,111,112,105,99,61,47,99,104,97,116,116,101,114,
                    // md5sum length
					39,0,0,0,
                        // md5sum=992ce8a1687cec8c8bd883ec73ca41d1
						109,100,53,115,117,109,61,57,57,50,99,101,56,97,49,54,56,55,99,101,99,56,99,56,98,100,56,56,51,101,99,55,51,99,97,52,49,100,49,
                    // type length
					20,0,0,0,
                        // type=std_msgs/String
                        116,121,112,101,61,115,116,100,95,109,115,103,115,47,83,116,114,105,110,103
            };

            var ms = new MemoryStream(msg);

            dynamic header = TcpRosHeaderSerializer.Deserialize(ms);
            
            var data = new std_msgs.String() { data = "test" };

            AssertEx.Is(header.callerid, "test");
            AssertEx.Is(header.topic,"/chatter");
            AssertEx.Is(header.md5sum,data.Md5Sum);
            AssertEx.Is(header.type, data.MessageType);

            ms.Close();
        }


        [TestMethod]
        public void Serialize_EmptyError()
        {
            var ms = new MemoryStream();

            var ex = AssertEx.Throws<RosTopicException>(() => TcpRosHeaderSerializer.Serialize(ms, new object()));
            ex.Message.Is("Header does not have properties");
        }

        [TestMethod]
        public void Deserialize_EmptyError()
        {
            var msg = new byte[0];

            var ms = new MemoryStream(msg);
            var ex = AssertEx.Throws<RosTopicException>(() => TcpRosHeaderSerializer.Deserialize(ms));
            ex.Message.Is("Stream length is too short");
        }

        [TestMethod]
        public void Deserialize_LengthMismatch()
        {
            var msg = new byte[] {100, 0, 0, 0, 0};

            var ms = new MemoryStream(msg);
            var ex = AssertEx.Throws<RosTopicException>(() => TcpRosHeaderSerializer.Deserialize(ms));
            ex.Message.Is("Stream length mismatch");
        }

        [TestMethod]
        public void Deserialize_LengthMismatch2()
        {
            var msg = new byte[] {4, 0, 0, 0, 1, 0, 0, 0};

            var ms = new MemoryStream(msg);
            var ex = AssertEx.Throws<RosTopicException>(() => TcpRosHeaderSerializer.Deserialize(ms));
            ex.Message.Is("Stream length mismatch");
        }
        [TestMethod]
        public void Deserialize_NotContainsEqual()
        {
            var msg = new byte[] {6, 0, 0, 0, 2, 0, 0, 0, 0, 0};

            var ms = new MemoryStream(msg);
            var ex = AssertEx.Throws<RosTopicException>(() => TcpRosHeaderSerializer.Deserialize(ms));
            ex.Message.Is("Header does not contain '='");
        }
    }

}
