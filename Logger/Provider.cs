using DrBAE.WdmServer.ExceptionProcessing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DrBAE.WdmServer.Logging
{
    public class Provider : ICheckParam<Provider>, ILoggerProvider
    {
        const string _logDir = "logs";
        public string FilePath { get; }

        readonly FileStream _fs;
        readonly StreamWriter _sw;
        object _lock = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public Provider(string filePath = "")
        {
            ///directory
            var dir = (Path.IsPathRooted(filePath) ? Path.GetDirectoryName(filePath) : _logDir) ?? _logDir;
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            //file name
            var fn = Path.GetFileName(filePath);
            if (string.IsNullOrWhiteSpace(fn)) fn = $"{DateTime.Now:yyMMdd-HHmmss.fff}.txt";

            //full path
            FilePath = Path.Combine(dir, fn);

            _fs = new FileStream(FilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            _sw = new StreamWriter(_fs);
        }

        public ILogger CreateLogger(string category)
        {
            ICheckParam<Provider>.CheckParam(nameof(CreateLogger), string.IsNullOrWhiteSpace(category), $"category=<{category}>이 유효하지 않다.", category);
            return new Logger(WriteLine, category);
        }

        public void WriteLine(string message)
        {
            lock (_lock)
            {
                _sw.WriteLine($"[{DateTime.Now.ToString("HHmmss.fff")}] {message}");
                _sw.Flush();
            }
        }

        public void Dispose()
        {
            _sw.Flush();
            _fs.Flush();
            _fs.Close();
            _fs.Dispose();
        }

    }//class
}
