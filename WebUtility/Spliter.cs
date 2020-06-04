using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using DrBAE.WdmServer.ExceptionProcessing;

namespace DrBAE.WdmServer.WebUtility
{
    public class Spliter : ICheckParam<Spliter>
    {
        public class M
        {
            public const string Split = nameof(Spliter.Split);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="numSplit"></param>
        /// <returns></returns>
        public static List<List<T>> Split<T>(List<T> source, int numSplit)
        {
            var message = $"numSplit가 source.Count보다 커서 처리 할 수 없습니다.";
            ICheckParam<Spliter>.CheckParam(M.Split, source.Count < numSplit, message, source, numSplit);

            message = "numSplit는 0보다 커야합니다.";
            ICheckParam<Spliter>.CheckParam(M.Split, 0 >= numSplit, message, source, numSplit);

            var Q = source.Count / numSplit;
            var R = source.Count % numSplit;

            var list = new List<List<T>>(numSplit);
            for (int d = 0, size; d < source.Count; d += size)
            {
                size = d < (Q + 1) * R ? Q + 1 : Q;
                list.Add(source.GetRange(d, size));
            }
            return list;
        }
    }//class
}
