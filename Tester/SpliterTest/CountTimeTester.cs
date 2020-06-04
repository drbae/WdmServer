using System;
using Xunit;
using System.Collections.Generic;
using System.Linq;

using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using DrBAE.WdmServer.WebUtility;
using System.Diagnostics;

namespace Tester.SplitTest
{
    public class SplitTester : TesterBase<SplitTester>
    {

        #region ==== List 요소 Count 테스트, List 요소 값 테스트 ====
        [Theory]
        [InlineData(9, new int[] { 3, 2, 2, 2 })]
        [InlineData(15, new int[] { 5, 5, 5 })]
        [InlineData(15, new int[] { 4, 4, 4, 3 })]
        [InlineData(15, new int[] { 3, 3, 3, 3, 3, })]
        [InlineData(15, new int[] { 3, 3, 3, 2, 2, 2 })]
        [InlineData(1123458, new int[] { 140433, 140433, 140432, 140432, 140432, 140432, 140432, 140432 })]
        public void numElement(int dataLength, int[] splitLenths)
        {
            var src = Enumerable.Range(0, dataLength).ToList();
            var list = src.Split(splitLenths.Length);

            //List가 입력된 숫자의 갯수만큼 나뉘어졌는지
            Assert.Equal(splitLenths.Length, list.Count);

            //나뉘어진 List의 요소의 총 Count가 원본가 일치하는지
            Assert.Equal(src.Count, list.Sum(x => x.Count));

            for (int i = 0; i < list.Count; i++) Assert.Equal(splitLenths[i], list[i].Count);
        }


        public static IEnumerable<object[]> TestData => new List<object[]>
        {
            new object[]{ 9,  new[]{ new [] { 0, 1, 2,}, new [] { 3,4 }, new [] { 5,6 }, new [] { 7,8 } }},
            new object[]{ 15, new[]{ new [] { 0, 1, 2, 3 }, new [] { 4, 5, 6,7 }, new [] {  8, 9, 10, 11 }, new [] { 12, 13, 14 } }},
            new object[]{ 15, new[]{ new [] { 0, 1, 2, 3, 4 }, new [] { 5, 6, 7, 8, 9 }, new [] { 10, 11, 12, 13, 14 } }},
            new object[]{ 15, new[]{ new [] { 0,1,2}, new[] {3,4,5 }, new[] {6,7,8 }, new[] {9,10,11 }, new[] { 12,13,14}, }},
            new object[]{ 15, new[]{ new [] { 0,1,2}, new[] {3,4,5 }, new[] {6,7,8 }, new[] {9,10 }, new[] {11,12 }, new[] {13,14 }, }}
        };


        [Theory]
        [MemberData(nameof(TestData))]
        public void Test(int dataLength, int[][] expData)
        {
            var exp = expData.Select(x => x.ToList()).ToList();
            var src = Enumerable.Range(0, dataLength).ToList();
            var list = src.Split(exp.Count);
            Assert.Equal(exp, list);

            for (int i = 0; i < list.Count; i++) Assert.Equal(exp[i], list[i]);
            for (int i = 0; i < list.Count; i++) for (int j = 0; j < list[i].Count; j++) Assert.Equal(exp[i][j], list[i][j]);//모든원소검사
        }
        #endregion


        #region ==== 시간 테스트 ====
        [Theory]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        public void splitSubTester(int loop)
        {
            var src = Enumerable.Range(0, 10000).ToList();
            var numLoop = Math.Pow(10, loop);
            var watch = Stopwatch.StartNew();
            for (int i = 0; i < numLoop; i++)
            {
                Split2(src, 8);
            }
            File.AppendAllText(@"SplitTest.txt", $"numLoop\t{numLoop}\ttime\t{watch.ElapsedMilliseconds}\tms\n");
        }


        public List<List<T>> Split1<T>(List<T> source, int numSplit)
        {
            var Q = source.Count / numSplit;
            var R = source.Count % numSplit;

            var list = new List<List<T>>(numSplit);
            for (int d = 0, size; d < source.Count; d += size)
            {
                size = d < (Q + 1) * R ? Q + 1 : Q;
                list.Add(source.GetRange(d, size));
            }
            return list;
        }

        public List<List<T>> Split2<T>(List<T> source, int numSplit)
        {
            var Q = source.Count / numSplit;
            var R = source.Count % numSplit;

            int d, size = 0;
            var list = new List<List<T>>(numSplit);
            for (d = 0; d < R; d += size)
            {
                size = Q + 1;
                list.Add(source.GetRange(d, size));
            }
            for (d = R; d < source.Count; d += size)
            {
                size = Q;
                list.Add(source.GetRange(d, size));
            }
            return list;
        } 
        #endregion

    }

}
