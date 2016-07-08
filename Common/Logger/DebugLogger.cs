using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Logger
{
    public class DebugLogger : ILogger
    {
        private void WriteLine(string message) 
        {
            System.Diagnostics.Debug.Write(message);
        }
        public void Debug(string message)
        {
            WriteLine(message);
        }
        public void Info(string message)
        {
            WriteLine(message);
        }
        public void Warning(string message)
        {
            WriteLine(message);
        }
        public void Error(string message)
        {
            WriteLine(message);
        }
        public void Fatal(string message)
        {
            WriteLine(message);
        }
    }
}
