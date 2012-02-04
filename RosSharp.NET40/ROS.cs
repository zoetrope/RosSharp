using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosSharp
{
    public class ROS
    {
        public static void Initialize()
        {
        }

        public static INode CreateNode()
        {
            return new RosNode();
        }

    }
}
