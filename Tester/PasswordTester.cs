using System;
using System.Collections.Generic;
using System.Text;
using DrBAE.WdmServer.WebUtility;
using Xunit;

namespace Tester
{
    public class PasswordTester : TesterBase<PasswordTester>
    {
        [Fact]
        public void run()
        {
            var exp = "my~Pwd^^!";
            var encoded = EmailOptions.Encode(exp);//이 값을 appsettings.json에 저장

            var o = new EmailOptions();
            o.Password = encoded;// appsettings.json에서 읽어들인 값을 지정

            Assert.Equal(exp, o.Password);
        }

    }//class
}
