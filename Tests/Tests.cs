using System;
using System.Linq;
using System.Net;
using System.IO;
using NUnit.Framework;
using QR_code_generator;

namespace Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Test_BoolArrtoBin()
        {
            bool[] test = {true, false, true, true};
            string res = "1011";
            string obtained = QRCode.BoolArrToBinString(test);
            Assert.AreEqual(obtained, res);
        }
        [Test]
        public void Test_BinStrToBoolArr()
        {
            bool[] test = {true, false, true, true};
            string res = "1011";
            var obtained = QRCode.BinStringToBoolArr(res);
            Assert.AreEqual(obtained, test);
        }
        [Test]
        public void Test_Encode()
        {
            var test = "HELLO WORLD";
            var a = QRCode.Encode(test);
            var b = "0110000101101111000110100010111001011011100010011010100001101";
            Assert.AreEqual(b, a);
        }
    }
}