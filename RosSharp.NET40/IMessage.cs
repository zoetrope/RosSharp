using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosSharp
{
    public interface IMessage
    {
        string DataType { get; }
        string Md5Sum { get; }
    }
}
