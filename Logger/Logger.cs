using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DrBAE.WdmServer.ExceptionProcessing;
using System.ComponentModel;

namespace DrBAE.WdmServer.Logging
{
    public class Logger : ICheckParam<Logger>, ILogger
    {
        readonly string _category;
        readonly Action<string> _write;
        const string _logHeader = "TDIWECN";

        /// <summary>
        /// 주어진 Action을 호출하여 주어진 category로 기록하는 로거를 생성한다.
        /// </summary>
        /// <param name="writeAction"></param>
        /// <param name="category"></param>
        public Logger(in Action<string> writeAction, in string category)
        {
            ICheckParam<Logger>.CheckParam(nameof(Logger), string.IsNullOrWhiteSpace(category), $"category=<{category}>이 유효하지 않다.", writeAction, category);

            _write = writeAction;
            _category = category;
        }

        /// <summary>
        /// TODO: 이것이 무슨 용도인지 파악할 것!
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="state"></param>
        /// <returns></returns>
        public IDisposable BeginScope<TState>(TState state)
        {
            return new D();
        }
        class D : IDisposable { public void Dispose() { } }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _write($"[{eventId:D02}] [{_logHeader[(int)logLevel]}] [{_category}] {formatter?.Invoke(state, exception) ?? $"{state}~{exception}"}");
        }

    }//class

}
