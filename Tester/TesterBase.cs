using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using DrBAE.WdmServer.Logging;

namespace Tester
{
    /// <summary>
    /// Tester 공통 사용 기능
    /// ▶Log기록 : 실행폴더\logs\TeesterLog 폴더에 현재날짜_{category}.txt 로그파일 생성 후 기록
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TesterBase<T>
    {
        const string _logDir = "TesterLog";
        readonly ILogger _logger;
        readonly string _logPath;

        /// <summary>
        /// 현재 시각과 테스터 클래스명을 조합한 파일명으로 로거 생성
        /// </summary>
        public TesterBase()
        {
            if (!Directory.Exists(_logDir)) Directory.CreateDirectory(_logDir);
            var fn = Path.Combine(_logDir, $"{DateTime.Now:HHmmss}_{typeof(T).Name}.txt");

            var provider = new Provider(fn);
            _logPath = provider.FilePath;
            _logger = provider.CreateLogger(typeof(T).Name);
        }

        protected void openLogFile()
        {
            new Process { StartInfo = new ProcessStartInfo(_logPath) { UseShellExecute = true } }.Start();
        }

        #region ---- Log 기록 ----

        /// <summary>
        /// 주어진 메시지와 호출메소드 이름을 ILogger.LogTrace() 로 기록
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="callingMethod"></param>
        protected void trace(object? msg, [CallerMemberName] string callingMethod = "")
        {
            _logger.LogTrace($"{callingMethod}(): {msg}");
        }

        #endregion


    }//class
}
