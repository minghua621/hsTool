using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

namespace Common.Logger
{
    public static class Log 
    {
        private static List<ILogger> loggers = new List<ILogger>();

        public static void Initialize(string LogPath)
        {
            loggers.Add(new DebugLogger());
            loggers.Add(new FileLogger(LogPath));
        }

        private static string GetFullMsg(string message, LoggerLevel level)
        {
            MethodBase method = new StackFrame(2).GetMethod();
            string clsName = method.ReflectedType.Name;
            string title = string.Format("{0} [{1}][{2}]", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), clsName, method.Name);
            return string.Format("{0}[{1}] {2}", title, level, message);
        }

        public static void Debug(string message)
        {
            foreach(ILogger logger in loggers)
            {
                logger.Debug(GetFullMsg(message, LoggerLevel.Debug));
            }
        }
        public static void Info(string message)
        {
            foreach (ILogger logger in loggers)
            {
                logger.Info(GetFullMsg(message, LoggerLevel.Info));
            }
        }
        public static void Warning(string message)
        {
            foreach (ILogger logger in loggers)
            {
                logger.Warning(GetFullMsg(message, LoggerLevel.Warning));
            }
        }
        public static void Error(string message)
        {
            foreach (ILogger logger in loggers)
            {
                logger.Error(GetFullMsg(message, LoggerLevel.Error));
            }
        }
        public static void Fatal(string message)
        {
            foreach (ILogger logger in loggers)
            {
                logger.Fatal(GetFullMsg(message, LoggerLevel.Fatal));
            }
        }
    }
}
