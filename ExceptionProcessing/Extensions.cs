using System;
using System.Collections.Generic;
using System.Text;

namespace DrBAE.WdmServer.ExceptionProcessing
{
    public static class Extensions
    {
        public static void CheckParam<T>(this T sender, string caller, bool tester, string format, params object[] args) where T : ICheckParam<T>, new()
        {
            ICheckParam<T>.CheckParam(caller, tester, format, args) ;
        }

    }//class
}
