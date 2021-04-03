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
        [Fact]
        void AnalysisDataModel()
        {
            var exp = new object?[] { 1, "abcd-xyz-123", "loss=1.23", 345, null, 543, null };
            var model = new AnalysisDataModel();
            model.SetValues(exp);
            var act = model.GetValues();
            Assert.Equal(exp, act);
        }
    }
}
