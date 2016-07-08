using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Common.Logger
{
    public class FileLogger : ILogger
    {
        private string LogPath = string.Empty;

        public FileLogger(string path)
        {
            LogPath = path;
            string dir = Path.GetDirectoryName(path);
            if(!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
        private void WriteLine(string message)
        {
            using (StreamWriter writer = new StreamWriter(LogPath, true))
            {
                writer.WriteLine(message);
            }
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
