using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.IO;
using NUnit.Framework;
using QR_code_generator;
using ReedSolomon;
using System.Text;

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
            string obtained = ArrayOps.BoolArrToBinString(test);
            Assert.AreEqual(obtained, res);
        }
        [Test]
        public void Test_BinStrToBoolArr()
        {
            bool[] test = {true, false, true, true};
            string res = "1011";
            var obtained = ArrayOps.BinStringToBoolArr(res);
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

        [Test]
        public void Test_BinStrToBytes()
        {
            var a = "0010011111001101001011100110100110010001";
            var exp = new byte[] {39,205,46,105,145};
            CollectionAssert.AreEqual(exp,ArrayOps.BinStrToBytes(a));
        }

        [Test]
        public void Test_ErrorCorrection()
        {
            var act = ReedSolomonAlgorithm.Encode(new byte[]
                {67, 85, 70, 134, 87, 38, 85, 194, 119, 50, 6, 18, 6, 103, 38},18,ErrorCorrectionCodeType.QRCode);
            
            var exp = new byte[] {213 ,199 ,11 ,45 ,115 ,247 ,241 ,223 ,229 ,248 ,154 ,117 ,154,111 ,86 ,161 ,111 ,39};
            
            CollectionAssert.AreEqual(exp,act);
        }
        
        [Test]
        public void Test_ByteArrToBinStr()
        {
            var act = ArrayOps.BytesToBinStr(new byte[] {67, 85, 70, 134});

            var exp = "01000011010101010100011010000110";
            
            CollectionAssert.AreEqual(exp,act);
        }
        
    }
}