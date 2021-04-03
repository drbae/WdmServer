using System;
using Xunit;
using DrBAE.WdmServer.Models;
using System.Diagnostics;
using Universe.Web.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Tester.Models
{
    public class LotTester
    {
        public class M1 : LotModel
        {
            [Fact] void type() => Assert.Equal(typeof(LotModel), ModelType);

            [Fact]
            void propCount()
            {
                Assert.Equal("Id", pId);
                Assert.Equal(3, DisplayNames.Count);
                Assert.Equal(3, PropNames.Count);
            }
            [Fact]
            void getValues()
            {
                var values = new object[] { 333, "FirstLot", DateTime.Now };
                var m = new M1() { Id = (int)values[0], LotName = (string)values[1], LotDate = (DateTime)values[2] };
                Assert.Equal(m.Id, values[0]);
                Assert.Equal(m.LotName, values[1]);
                Assert.Equal(m.LotDate, values[2]);
                Assert.Equal(3, GetValues().Count);
            }

        }

    }
}
