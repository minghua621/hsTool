using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

namespace Common.Logger
{
    public class Log : ILogger
    {
        private List<ILogger> loggers = new List<ILogger>();

        public static string LogPath = string.Empty;

        public static Log Logger
        {
            get { return logger = logger ?? new Log(); }
        }
        private static Log logger;

        public Log()
        {
            loggers.Add(new DebugLogger());
            loggers.Add(new FileLogger(LogPath));
        }

        private string GetFullMsg(string message, LoggerLevel level)
        {
            MethodBase method = new StackFrame(2).GetMethod();
            string clsName = method.ReflectedType.Name;
            string title = string.Format("{0} [{1}][{2}]", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), clsName, method.Name);
            return string.Format("{0}[{1}] {2}", title, level, message);
        }

        public void Debug(string message)
        {
            foreach(ILogger logger in loggers)
            {
                logger.Debug(GetFullMsg(message, LoggerLevel.Debug));
            }
        }
        public void Info(string message)
        {
            foreach (ILogger logger in loggers)
            {
                logger.Info(GetFullMsg(message, LoggerLevel.Info));
            }
        }
        public void Warning(string message)
        {
            foreach (ILogger logger in loggers)
            {
                logger.Warning(GetFullMsg(message, LoggerLevel.Warning));
            }
        }
        public void Error(string message)
        {
            foreach (ILogger logger in loggers)
            {
                logger.Error(GetFullMsg(message, LoggerLevel.Error));
            }
        }
        public void Fatal(string message)
        {
            foreach (ILogger logger in loggers)
            {
                logger.Fatal(GetFullMsg(message, LoggerLevel.Fatal));
            }
        }
    }
}
