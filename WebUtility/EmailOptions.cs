using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DrBAE.TnM.Utility;

namespace DrBAE.WdmServer.WebUtility
{
    public class EmailOptions
    {
        static Encoding _encoding = System.Text.Encoding.ASCII;

        public string MailServer { get; set; } = string.Empty;
        public int MailPort { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string Sender { get; set; } = string.Empty;

        /// <summary>
        /// 인코딩된 문자열을 넣으면 디코딩된 문자열이 리턴됨
        /// </summary>
        public string Password
        {
            get => _password;
            set
            {
                _hexaPassword = value;
                _password = Decode(value);
            }
        }
        string _hexaPassword = string.Empty;
        string _password = string.Empty;

        /// <summary>
        /// 문자열을 인코딩된 문자열로 
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Encode(string plainText) => _encoding.GetBytes(plainText).Select(x => (byte)~x).ToHexaString();

        /// <summary>
        /// 인코딩된 문자열을 디코딩된 문자열로
        /// </summary>
        /// <param name="encodedText"></param>
        /// <returns></returns>
        public static string Decode(string encodedText) => _encoding.GetString(encodedText.FromHexaString().Select(x => (byte)~x).ToArray());

    }//class
}
