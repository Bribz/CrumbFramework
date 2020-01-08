using System;
using System.Collections.Generic;
using System.Text;

namespace CrumbShared
{
    public class Log
    {
        public delegate void LogEvent(LogLevel level, string msg);
        public static event LogEvent LogReceived;

        public static int Current_LogLevel { get; private set; } = (int)LogLevel.SYSTEM;

        public static void ChangeLogLevel(LogLevel level)
        {
            Current_LogLevel = (int)level;
        }

        public static void LogSystem(string message, params object[] list)
        {
            LogMsg(LogLevel.SYSTEM, message, list);
        }

        public static void LogVerbose(string message, params object[] list)
        {
            LogMsg(LogLevel.DEBUG_VERBOSE, message, list);
        }

        public static void LogWarning(string message, params object[] list)
        {
            LogMsg(LogLevel.WARNING, message, list);
        }

        public static void LogError(string message, params object[] list)
        {
            LogMsg(LogLevel.ERROR, message, list);
        }

        /// <summary>
        /// Log message to console. Defaults to log level of DEBUG
        /// </summary>
        /// <param name="message"></param>
        /// <param name="list"></param>
        public static void LogMsg(string message, params object[] list)
        {
            LogMsg(LogLevel.DEBUG, message, list);
        }

        /// <summary>
        /// Log message of any kind.
        /// </summary>
        /// <param name="level">Log Level</param>
        /// <param name="message">Message with formatters</param>
        /// <param name="list">Object parameter list for formatted strings</param>
        public static void LogMsg(LogLevel level, string message, params object[] list)
        {
            if (((int)level) > Current_LogLevel)
            {
                return;
            }

            string msg = message;
            if (list != null && list.Length > 0)
            {
                msg = string.Format(message, list);
            }

            if (LogReceived != null)
            {
                LogReceived(level, msg);
            }
        }
    }

    public enum LogLevel
    {
        NONE,
        SYSTEM,
        ERROR,
        WARNING,
        DEBUG,
        DEBUG_VERBOSE,
        ALL
    }
}
