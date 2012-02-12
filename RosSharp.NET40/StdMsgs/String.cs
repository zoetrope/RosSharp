using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosSharp.StdMsgs
{
    public class String : IMessage
    {
        public string DataType
        {
            get { return "std_msgs/String"; }
        }

        public string Md5Sum
        {
            get { return "992ce8a1687cec8c8bd883ec73ca41d1"; }
        }

        public string Data { get; set; }
    }
}
