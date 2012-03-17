using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Transport;

namespace RosSharp.Tests.Transport
{
    [TestClass]
    public class TcpRosHeaderTest
    {
        [TestMethod]
        public void SerializeSubscriberHeader_Success()
        {
            var serializer = new TcpRosHeaderSerializer<SubscriberHeader>();

            var data = new StdMsgs.String() { data = "test" };

            var header = new SubscriberHeader()
            {
                callerid = "test",
                topic = "/chatter",
                md5sum = data.Md5Sum,
                type = data.MessageType
            };

            var ms = new MemoryStream();

            serializer.Serialize(ms, header);

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
            var serializer = new TcpRosHeaderSerializer<SubscriberHeader>();

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

            var header = serializer.Deserialize(ms);
            
            var data = new StdMsgs.String() { data = "test" };

            header.callerid.Is("test");
            header.topic.Is("/chatter");
            header.md5sum.Is(data.Md5Sum);
            header.type.Is(data.MessageType);

            ms.Close();
        }
    }

}
