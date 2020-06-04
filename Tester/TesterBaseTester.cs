using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Tester
{
    public class TesterBaseTester : TesterBase<TesterBaseTester>
    {
        [Fact]
        public void run()
        {
            trace("a test log message from TesterBaseTester.run()");
            openLogFile();
        }

    }//class
}
