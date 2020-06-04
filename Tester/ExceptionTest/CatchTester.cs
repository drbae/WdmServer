using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using DrBAE.WdmServer.WebUtility;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Net;
using DrBAE.WdmServer.ExceptionProcessing;

namespace Tester.ICheckParamTest
{
    public class CatchTester : TesterBase<CatchTester>
    {
        [Theory]
        [InlineData(7, 8)]//numSplit이sourceCount보다큼 //예외를 던지고 테스트 통과해야함
        [InlineData(10000, 0)]//numSplit이0 //예외를 던지고 테스트 통과해야함
        public void 예외처리테스트(int srcLength, int numSplit)
        {
            var src = Enumerable.Range(0, srcLength).ToList();
            try
            {
                Spliter.Split(src, numSplit);
            }
            catch (Exception ex) when (ex.Data.Contains(Spliter.M.Split))
            {
                trace(ex.Message);
                foreach (var item in (IEnumerable<object>)(ex.Data[Spliter.M.Split] ?? new object[0]))
                {
                    var value = ((object, object))item;
                    if (value.Item2 is ICollection) trace((value.Item1, (value.Item2, ((ICollection)value.Item2).Count)));
                    else trace(item);
                }
            }
        }
    }//class
}
