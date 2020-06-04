using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace DrBAE.WdmServer.ExceptionProcessing
{
    /// <summary>
    /// Exception.Data에 주어진 키를 추가하고, Exception을 throw 한다.
    /// </summary>
    public class ExBuilder : Exception
    {
        //public ExBuilder() : this("", "") { }
        public ExBuilder(string key, string exMessage) : base(exMessage)
        {
            //Message = exMessage;
            Key = key;
            Values = new List<object>();
        }
        public static ExBuilder Create(string exMessage, [CallerMemberName] string key = "") => new ExBuilder(key, exMessage);

        public string Key { get; }
        //public string Message { get; }

        public List<object> Values { get; }
        public void AddData(object value) => Values.Add(value);
        public void AddData(IEnumerable<object> names, IEnumerable<object> values)
        {
            var len = Math.Min(names.Count(), values.Count());
            for (int i = 0; i < len; i++) Values.Add((names.ElementAt(i), values.ElementAt(i)));
        }
        public ExBuilder Throw()
        {
            var ex = new Exception(Message);
            ex.Data.Add(Key, Values);
            throw ex;
        }
    }
}
