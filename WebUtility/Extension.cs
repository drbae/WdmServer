using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DrBAE.WdmServer.WebUtility
{
    public static class Extension
    {
        public static List<List<T>> Split<T>(this List<T> source, int numSplit) => Spliter.Split(source, numSplit);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logic"></param>
        /// <returns></returns>
        public static string GetVersion(this object logic)
        {
            var version = logic.GetType().Assembly.GetName().Version ?? new Version();
            //return version.Build + version.Revision * (decimal)Math.Pow(10, -(int)Math.Log10(version.Revision) - 1);
            return version.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static (byte[], string) ToByte(this IFormFile file)
        {
            using (var msPigtailData = new MemoryStream())
            {
                file.CopyTo(msPigtailData);
                return (msPigtailData.ToArray(), file.FileName);
            }
        }

        public static byte[] RemoveBOM(this string fileText) => RemoveBOM(Encoding.UTF8.GetBytes(fileText));

        public static byte[] RemoveBOM(this byte[] source)
        {
            byte[] dest = source;
            if (source[0] == 239)
            {
                dest = new byte[source.Length - 3];
                Array.Copy(source, 3, dest, 0, dest.Length);
            }
            return dest;
        }


    }//class

}
