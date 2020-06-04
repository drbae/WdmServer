using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using DrBAE.WdmServer.Logging;

namespace Tester.LoggerTest
{
    using L = Logger;

    public class LoggerTester
    {

        [Fact] void logFile()
        {
            const string _file = @"LoggerTester.logFile.txt";
            var provider = new Provider(_file);
            new Process { StartInfo = new ProcessStartInfo(provider.FilePath){ UseShellExecute = true } }.Start();

            provider = new Provider(@"D:\Neon\RnD\Project\www\WinServer\GitHub\App\WdmServer\WepApp\Doc\테스트\191106 Provider\");
            new Process { StartInfo = new ProcessStartInfo(provider.FilePath) { UseShellExecute = true } }.Start();
        }

        [Fact] void multiLogger()
        {
            var numLogger = 8;
            var numEvent = 1000;
            var provider = new Provider();

            var workers = new Task[numLogger];
            for (int i = 0; i < numLogger; i++)
            {
                var id = i;
                workers[i] = Task.Run(() => 
                {
                    var random = new Random(id);
                    for (int j = 0; j < numEvent; j++)
                    {
                        Thread.Sleep(random.Next(1, 10));
                        provider.WriteLine($"{id} {j}");
                    }
                });
            }

            Task.WaitAll(workers);
            provider.Dispose();

            var numLines = File.ReadAllText(provider.FilePath).Split("\r\n").Length;
            Assert.Equal(numLogger * numEvent + 1, numLines);
            new Process { StartInfo = new ProcessStartInfo(provider.FilePath) { UseShellExecute = true } }.Start();
        }

        [Fact] void run()
        {
            var provider = new Provider();
            new Process { StartInfo = new ProcessStartInfo(provider.FilePath) { UseShellExecute = true } }.Start();

            var logger = (L)provider.CreateLogger(nameof(LoggerTester));

            var ex = new Exception("a test exception");
            logger.LogCritical("a critical message: {0}", ex);
            logger.LogError("a error message", ex);
            logger.LogWarning("a warning message");
            logger.LogInformation("a info message", ex);
            logger.LogDebug("a debug message", ex);
            logger.LogTrace("a trace message", ex);

            logger.Log(LogLevel.Information, new EventId(), this, null!, null!);
        }

        public override string ToString() => $"(state of the instance)";

        
    }//class
}
