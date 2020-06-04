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
    public class ModelTester
    {
        public class M1 : ModelBase<M1> 
        {
            [Fact] void type() => Assert.Equal(typeof(M1), ModelType);
        }
        public class M2 : ConfigModel
        {
            [Fact]
            void type() => Assert.Equal(typeof(ConfigModel), ModelType);

            [Fact] void propCount()
            {
                Assert.Equal("Id", pId);
                Assert.Equal(7, M2.DisplayNames.Count);
                Assert.Equal(7, M2.PropNames.Count);
            }
            [Fact] void getValues()
            {
                var m = new ConfigModel() { Id = 1 };
                Assert.Equal(7, m.GetPropNames().Count);
                Assert.Equal(7, m.GetDisplayNames().Count);
                var v = GetValues();
                Assert.Equal(7, v.Count);
            }
            [Fact] void setValues()
            {
                //_propNames = new[] { nameof(Type), nameof(Name), nameof(Content), nameof(Pol), nameof(NumCh), nameof(Description) };
                var m = new ConfigModel();
                var exp = new object[] { 11, ConfigType.Analysis, "Test", "test contents", false, 5, "test config" };
                m.SetValues(exp);
                var act = m.GetValues();
                Assert.Equal(exp, act);
            }

            [Fact] void toString()
            {
                var m = new ConfigModel();
                var str = ToString();
                Debug.WriteLine(str);

                Assert.Equal(7, str.Split(',').Length);
            }
        }

    }
}
