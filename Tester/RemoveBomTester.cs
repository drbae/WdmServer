using System;
using System.Collections.Generic;
using System.Text;

namespace Tester
{
    public class RemoveBomTester
    {

        [Fact]
        public void RemoveBomByteTest()
        {
            var BomByte = File.ReadAllBytes(@"D:\WorkSpace\RawData\BOM Test\Test-BOM.txt");
            var NotBomByte = File.ReadAllBytes(@"D:\WorkSpace\RawData\BOM Test\Test-NoBOM.txt");

            Assert.Equal(239, BomByte[0]);
            Assert.Equal(187, BomByte[1]);
            Assert.Equal(191, BomByte[2]);

            Assert.NotEqual(239, NotBomByte[0]);
            Assert.NotEqual(187, NotBomByte[1]);
            Assert.NotEqual(191, NotBomByte[2]);

            var RemoveBomText = Utill.RemoveBOM(BomByte);//Byte파일을 Text로 변환하면서 BOM제거
            var RemoveBomByte = Encoding.UTF8.GetBytes(RemoveBomText);
            Assert.Equal(NotBomByte, RemoveBomByte);
        }

        [Fact]
        public void RemoveBomTextTest()
        {
            var BomByte = File.ReadAllBytes(@"D:\WorkSpace\RawData\BOM Test\Test-BOM.txt");
            var NotBomByte = File.ReadAllBytes(@"D:\WorkSpace\RawData\BOM Test\Test-NoBOM.txt");

            Assert.Equal(239, BomByte[0]);
            Assert.Equal(187, BomByte[1]);
            Assert.Equal(191, BomByte[2]);

            Assert.NotEqual(239, NotBomByte[0]);
            Assert.NotEqual(187, NotBomByte[1]);
            Assert.NotEqual(191, NotBomByte[2]);

            var BomText = Encoding.UTF8.GetString(BomByte);
            var RemoveBomByte = Utill.RemoveBOM(BomText);//Text파일을 Byte로 변환하면서 BOM제거
            Assert.Equal(NotBomByte, RemoveBomByte);
        }
    }
}
