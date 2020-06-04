using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit;
using DrBAE.WdmServer.WebUtility;
using Xunit.Abstractions;
using System.Text;

namespace Tester
{
    public class NullableTester : TesterBase<NullableTester>
    {
        [Fact]
        public void source가null일때테스트()
        {
            //var testee = new Testee(); Testee.Split는 null 파라미터를 받을 수 없기 때문에 테스트 불가능
            List<int> src = null!;

            //src = new List<int>(); //할당하지 않으면 Split의 파라미터 src에서 에러발생
            Assert.Throws<ArgumentNullException>(() => src ?? throw new ArgumentNullException()/*testee.Split(src, 8)*/);//예외를 던지고 테스트 통과해야함

            //Split의 파라미터가 null일경우 Complier에러가 발생하므로 테스트 할 필요가 없다.
        }

        [Theory]
        [InlineData(7, 8, typeof(Testee.ExSource))]//numSplit이sourceCount보다큼 //예외를 던지고 테스트 통과해야함
        [InlineData(10000, 0, typeof(Testee.ExNumSplit))]//numSplit이0 //예외를 던지고 테스트 통과해야함
        public void 예외를_던지는_경우(int sourceCount, int numSplit, Type exType)
        {
            var testee = new Testee();
            var src = Enumerable.Range(0, sourceCount).ToList();
            var ex = Assert.Throws(exType, () => testee.Run(src, numSplit));
            trace(ex);
        }

        [Theory]
        [InlineData(10000, 8)]//sourceCount가numSplit보다큼 //예외를 던지지 않고 테스트 실패해야함
        [InlineData(8, 8)]//sourceCount와numSplit이같음 //예외를 던지지 않고 테스트 실패해야함
        public void 정상_작동하는_경우(int sourceCount, int numSplit)
        {
            var testee = new Testee();
            var src = Enumerable.Range(0, sourceCount).ToList();
            testee.Run(src, numSplit);
        }

        public class Testee
        {
            public void Run(List<int> source, int numSplit)
            {
                입력검증(source, numSplit);
            }

            private static void 입력검증<T>(List<T> source, int numSplit)
            {
                //if (source == null) throw new ArgumentNullException($"source : {source} source가 null이여서 처리 할 수 없습니다.");
                if (source.Count < numSplit) throw new ExSource($"numSplit : {numSplit} source.Count : {source.Count} numSplit가 source.Count보다 커서 처리 할 수 없습니다.");
                if (numSplit <= 0) throw new ExNumSplit($"numSplit : {numSplit} numSplit는 0보다 커야합니다.");
            }

            public class ExNumSplit : Exception { public ExNumSplit(string message) : base(message) { } }
            public class ExSource : Exception { public ExSource(string message) : base(message) { } }
        }

    }//class


}
