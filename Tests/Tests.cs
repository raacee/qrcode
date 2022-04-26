using System;
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
    }
}