using System;

namespace ProdeusConverter
{
    //Just a very simple static object that passes trough log events
    static class Logger
    {
        //Types
        public delegate void LogDelegate(LogType logType, string message);

        public enum LogType
        {
            Normal,
            Warning,
            Error
        }

        //Event
        public static event LogDelegate MessagedLoggedEvent = null;

        //Utility
        public static void LogMessage(LogType logType, string message)
        {
            if (MessagedLoggedEvent != null)
                MessagedLoggedEvent(logType, message);

            #if DEBUG
                Console.WriteLine(message);
            #endif
        }

        public static void LogMessage(string message)
        {
            LogMessage(LogType.Normal, message);
        }

        public static void LogWarning(string message)
        {
            LogMessage(LogType.Warning, message);
        }

        public static void LogError(string message)
        {
            LogMessage(LogType.Error, message);
        }
    }
}
