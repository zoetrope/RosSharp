using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Common.Logging;
using Common.Logging.Simple;

namespace RosSharp
{

    public class RosOutLoggerFactoryAdapter : AbstractSimpleLoggerFactoryAdapter
    {
        public RosOutLoggerFactoryAdapter()
            : base(null)
        {
        }
        public RosOutLoggerFactoryAdapter(NameValueCollection properties)
            : base(properties)
        {
        }

        protected override ILog CreateLogger(string name, LogLevel level, bool showLevel, bool showDateTime,
                                             bool showLogName, string dateTimeFormat)
        {
            return new RosOutLogger(name, level, showLevel, showDateTime, showLogName, dateTimeFormat);
        }
    }

    public class RosOutLogger : AbstractSimpleLogger
    {
        public RosOutLogger(string logName, LogLevel logLevel, bool showLevel, bool showDateTime, bool showLogName,
                        string dateTimeFormat)
            : base(logName, logLevel, showLevel, showDateTime, showLogName, dateTimeFormat)
        {
        }

        protected override void WriteInternal(LogLevel level, object message, Exception e)
        {
            var sb = new StringBuilder();
            FormatOutput(sb, level, message, e);

            //TODO: RosOutに出力するように。
            Console.Out.WriteLine(sb.ToString());
        }
    }
}
