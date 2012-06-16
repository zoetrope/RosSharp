using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Common.Logging;

namespace RosSharp.Utility
{
    public static class RosOutLogManager
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static ILog GetCurrentNodeLogger(string nodeId)
        {
            var sf = new StackFrame(1, false);
            var callerType = sf.GetMethod().DeclaringType;
            string name = callerType + "@" + nodeId;

            return LogManager.Adapter.GetLogger(name);
        }
    }
}
