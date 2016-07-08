using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Logger
{
    public enum LoggerLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Fatal,
    }

    public interface ILogger
    {
        void Debug(string message);
        void Info(string message);
        void Warning(string message);
        void Error(string message);
        void Fatal(string message);      
    }
}
