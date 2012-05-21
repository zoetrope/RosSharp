using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using RosSharp.rosgraph_msgs;

namespace RosSharp.Utility
{
    internal static class LogLevelExtensions
    {
        public static bool TryParse(string level, out byte ret)
        {
            switch (level)
            {
                case "DEBUG":
                    ret = Log.DEBUG;
                    break;
                case "INFO":
                    ret = Log.INFO;
                    break;
                case "WARN":
                    ret = Log.WARN;
                    break;
                case "ERROR":
                    ret = Log.ERROR;
                    break;
                case "FATAL":
                    ret = Log.FATAL;
                    break;
                default:
                    ret = 0;
                    return false;
            }
            return true;
        }

        public static LogLevel ToLogLevel(this byte level)
        {
            LogLevel ret;
            switch (level)
            {
                case Log.DEBUG:
                    ret = LogLevel.Debug;
                    break;
                case Log.INFO:
                    ret = LogLevel.Info;
                    break;
                case Log.WARN:
                    ret = LogLevel.Warn;
                    break;
                case Log.ERROR:
                    ret = LogLevel.Error;
                    break;
                case Log.FATAL:
                    ret = LogLevel.Fatal;
                    break;
                default:
                    throw new ArgumentException();
            }
            return ret;

        }
        public static byte ToLogLevel(this LogLevel level)
        {
            byte ret;
            switch (level)
            {
                case LogLevel.Debug:
                    ret = Log.DEBUG;
                    break;
                case LogLevel.Info:
                    ret = Log.INFO;
                    break;
                case LogLevel.Warn:
                    ret = Log.WARN;
                    break;
                case LogLevel.Error:
                    ret = Log.ERROR;
                    break;
                case LogLevel.Fatal:
                    ret = Log.FATAL;
                    break;
                default:
                    throw new ArgumentException();
            }
            return ret;
        }


        public static string ToLogLevelString(this byte level)
        {
            string ret;
            switch (level)
            {
                case Log.DEBUG:
                    ret = "DEBUG";
                    break;
                case Log.INFO:
                    ret = "INFO";
                    break;
                case Log.WARN:
                    ret = "WARN";
                    break;
                case Log.ERROR:
                    ret = "ERROR";
                    break;
                case Log.FATAL:
                    ret = "FATAL";
                    break;
                default:
                    throw new ArgumentException();
            }
            return ret;
        }
    }
}
