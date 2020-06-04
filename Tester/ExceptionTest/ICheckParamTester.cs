using DrBAE.WdmServer.WebUtility;
using System;
using System.Collections.Generic;
using System.Linq;  
using System.Text;
using Xunit;
using DrBAE.WdmServer.ExceptionProcessing;
using System.Reflection;

namespace Tester.ICheckParamTest
{
    public class ICheckParamTester
    {
        class C1 : ICheckParam<C1> 
        { 
            public static void Run1() => ICheckParam<C1>.CheckParam(nameof(Run1), true, "이것은 의도된 예외이다.");
            public static void Run2(int a) => ICheckParam<C2>.CheckParam(nameof(Run2), true, "", a);
            public static void Run2(decimal a) => ICheckParam<C2>.CheckParam(nameof(Run2), true, "", a);

            public static Dictionary<string, IEnumerable<MethodBase>> MI => ICheckParam<C1>._mi;
        }

        [Fact] public void C1_Run1_retister()
        {            
            try
            {
                C1.Run1();
            }
            catch (Exception ex) when (ex.Data.Contains(nameof(C1.Run1)))
            {
                return;
            }
            Assert.True(false);
        }

        [Fact] void C1_register()
        {
            Assert.Equal(4, C1.MI.Count);//Run1, Run2, get_MI, .ctor
            Assert.True(1 == C1.MI[nameof(C1.Run1)].Count());
            Assert.True(2 == C1.MI[nameof(C1.Run2)].Count());
            Assert.True(1 == C1.MI[nameof(C1)].Count());
        }

        class C2 : ICheckParam<C2> //
        {
            public static void Test(int a) => ICheckParam<C2>.CheckParam(nameof(Test), true, "", (double)a);
            public static void Test(decimal a) => ICheckParam<C2>.CheckParam(nameof(Test), true, "", a);
        }

        [Fact] public void 메소드_못찾음_예외발생()
        {
            Assert.Throws<InvalidOperationException>(()=> C2.Test(1));
        }


    }//class
}