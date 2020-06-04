using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DrBAE.WdmServer.ExceptionProcessing
{
    /// <summary>
    /// 주어진 타입 T의 메소드 목록을 생성, 호출 파리미터 정보를 Execption.Data에 추가, Exception을 throw 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICheckParam<T> where T : ICheckParam<T>
    {
        static readonly Dictionary<string, IEnumerable<MethodBase>> _mi;
        static Type _type;
        //static T _instance = new T();
        const BindingFlags _bf = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

        /// <summary>
        /// 모든 MethodInfo를 메소드 이름을 키로하는 사전에 등록
        /// </summary>
        static ICheckParam()
        {
            _type = typeof(T);
            _mi = new Dictionary<string, IEnumerable<MethodBase>>();

            var methods = _type.GetMethods(_bf);
            foreach (var mi in methods)
            {
                if (!_mi.ContainsKey(mi.Name)) _mi[mi.Name] = methods.Where(m => m.Name == mi.Name);
            }

            _mi[_type.Name] = _type.GetConstructors();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="tester"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void CheckParam(string caller, bool tester, string format, params object[] args)
        {
            if (!tester) return;

            var ext = new ExBuilder(caller, string.Format(format, args));
            ext.AddData(findParamNames(caller, args), args);
            ext.Throw();
        }
        static IEnumerable<string> findParamNames(string caller, params object[] args)
        {
            //if (_mi == null) throw new InvalidOperationException($"{nameof(ICheckParam<T>)}._mi 이 null입니다.");
            if (!_mi.ContainsKey(caller) || _mi[caller].Count() == 0) 
                throw new KeyNotFoundException($"{nameof(ICheckParam<T>)}._mi 에 {caller}메소드는 등록되지 않았다.");

            MethodBase? miFound = null;
            var list = _mi[caller];
            if (list.Count() == 1) miFound = list.First();
            else
            {
                foreach (var m in list)
                {
                    var matched = true;
                    var paramList = m.GetParameters();
                    for (int p = 0; p < args.Length; p++) matched &= paramList[p].ParameterType.Equals(args[p].GetType());
                    if (matched)
                    {
                        miFound = m;
                        break;
                    }
                }
            }
            if(miFound != null) return miFound.GetParameters().Select(x => x.Name ?? string.Empty);
            throw new InvalidOperationException($"caller = {caller}, args.Length = {args.Length} 인 메소드를 찾지 못함");
        }

        public static string ParamString(string caller, params object[] paramValues)
        {
            var parameters = findParamNames(caller, paramValues);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"---------- {caller}() ----------");
            for (int i = 0; i < paramValues.Length; i++) sb.AppendLine($"{(parameters.ElementAt(i), paramValues[i])}");
            return sb.ToString();
        }


    }//class
}
