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
        public void source��null�϶��׽�Ʈ()
        {
            //var testee = new Testee(); Testee.Split�� null �Ķ���͸� ���� �� ���� ������ �׽�Ʈ �Ұ���
            List<int> src = null!;

            //src = new List<int>(); //�Ҵ����� ������ Split�� �Ķ���� src���� �����߻�
            Assert.Throws<ArgumentNullException>(() => src ?? throw new ArgumentNullException()/*testee.Split(src, 8)*/);//���ܸ� ������ �׽�Ʈ ����ؾ���

            //Split�� �Ķ���Ͱ� null�ϰ�� Complier������ �߻��ϹǷ� �׽�Ʈ �� �ʿ䰡 ����.
        }

        [Theory]
        [InlineData(7, 8, typeof(Testee.ExSource))]//numSplit��sourceCount����ŭ //���ܸ� ������ �׽�Ʈ ����ؾ���
        [InlineData(10000, 0, typeof(Testee.ExNumSplit))]//numSplit��0 //���ܸ� ������ �׽�Ʈ ����ؾ���
        public void ���ܸ�_������_���(int sourceCount, int numSplit, Type exType)
        {
            var testee = new Testee();
            var src = Enumerable.Range(0, sourceCount).ToList();
            var ex = Assert.Throws(exType, () => testee.Run(src, numSplit));
            trace(ex);
        }

        [Theory]
        [InlineData(10000, 8)]//sourceCount��numSplit����ŭ //���ܸ� ������ �ʰ� �׽�Ʈ �����ؾ���
        [InlineData(8, 8)]//sourceCount��numSplit�̰��� //���ܸ� ������ �ʰ� �׽�Ʈ �����ؾ���
        public void ����_�۵��ϴ�_���(int sourceCount, int numSplit)
        {
            var testee = new Testee();
            var src = Enumerable.Range(0, sourceCount).ToList();
            testee.Run(src, numSplit);
        }

        public class Testee
        {
            public void Run(List<int> source, int numSplit)
            {
                �Է°���(source, numSplit);
            }

            private static void �Է°���<T>(List<T> source, int numSplit)
            {
                //if (source == null) throw new ArgumentNullException($"source : {source} source�� null�̿��� ó�� �� �� �����ϴ�.");
                if (source.Count < numSplit) throw new ExSource($"numSplit : {numSplit} source.Count : {source.Count} numSplit�� source.Count���� Ŀ�� ó�� �� �� �����ϴ�.");
                if (numSplit <= 0) throw new ExNumSplit($"numSplit : {numSplit} numSplit�� 0���� Ŀ���մϴ�.");
            }

            public class ExNumSplit : Exception { public ExNumSplit(string message) : base(message) { } }
            public class ExSource : Exception { public ExSource(string message) : base(message) { } }
        }

    }//class


}
