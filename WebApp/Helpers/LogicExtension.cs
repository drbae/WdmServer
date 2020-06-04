using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using DrBAE.TnM.Utility;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using DrBAE.WdmServer.WebUtility;
using Universe.DataAnalysis;
using Ko.Pigtail;
using DrBAE.TnM.WdmAnalyzer;

namespace DrBAE.WdmServer.WebApp
{
    public static class LogicExtension
    {
        public static IAnalyzer LoadAnalyzer(this IServiceProvider sp, TypeOfDUT dutType)
        {
            return new AnalyzerClient(dutType);//
        }

        public static IRawLogic LoadRawLogic(this IServiceProvider sp, string? rawConfig)
        {
            return new RawLogic(rawConfig?.RemoveBOM());
        }

        public static IReportLogic LoadReportLogic(this IServiceProvider sp, string? reportConfig)
        {
            return new ReportLogic(reportConfig?.RemoveBOM());
        }

    }//class

}
