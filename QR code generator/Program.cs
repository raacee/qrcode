using System;
using Projet_Info;
using System.IO;
using ReedSolomon;
using System.Diagnostics;
using System.Net;
using System.Xml.Serialization;

namespace QR_code_generator
{
    internal class Program
    {
        public static void Main()
        {
            QRCode hello_world = new QRCode("HellO WorLD");
            hello_world.Generate();
        }
    }

    public class QRCode
    {
        private byte version;
        private string binstr;
        private string data;
        private bool[] mode = {false, false, true, false};
        private char correction_mode;
        private bool[] length_bits;
        private bool[] encoded_data;
        private bool[] correctionbits;
        private bool[] data_binaries;
        private bool[] allbits;
        private Module[,] matrix;

        public QRCode(string text, char corrMode = 'L', byte v = 1)
        {
            text = text.ToUpper();
            this.data = text;
            this.version = v;
            binstr = ArrayOps.BoolArrToBinString(mode)+Convert.ToString(text.Length,2).PadLeft(9,'0');
            this.correction_mode = corrMode;
            this.encoded_data = ArrayOps.BinStringToBoolArr(Encode(text.ToUpper()));
            binstr += ArrayOps.BoolArrToBinString(this.encoded_data);
            
            //adding terminator and 0s
            for (int i = 0; i < 4; i++)
            {
                if (binstr.Length < 19 *8) binstr += "0";                
                else break;
            }
            while (binstr.Length % 8 != 0)
            {
                binstr += "0";
            }
            
            //adding pad bytes
            int n = (19 * 8 - binstr.Length) / 8;
            for (int i = 0; i < n; i++)
            {
                if (i % 2 == 0)
                {
                    binstr += "11101100";
                }
                else
                {
                    binstr += "00010001";
                }
            }
            data_binaries = ArrayOps.BinStringToBoolArr(binstr);
            this.correctionbits = ArrayOps.BytesToBoolArr(ErrorCorrection(binstr));
            binstr += ArrayOps.BoolArrToBinString(this.correctionbits);
            allbits = ArrayOps.BinStringToBoolArr(binstr);
            BuildTemplate('L');
        }

        private void BuildTemplate(char error_corr = 'L')
        {
            //set all modules to false
            this.matrix = new Module[21+4*(version-1),21+4*(version-1)];
            int length = 21 + 4 * (version-1);

            for (int i = 0; i < this.matrix.GetLength(0); i++)
            {
                for (int j = 0; j < this.matrix.GetLength(1); j++)
                {
                    this.matrix[i, j] = new Module(false);
                }
            }

            //adding the finder patterns
            Module[,] finderpattern = new Module[7, 7];
            
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (i == 0 || j == 0 || i == 6 || j == 6 ||
                        (i <= 4 && i >= 2 && j <= 4 && j >= 2))
                    {
                        finderpattern[i, j] = new Module(true,true,true);
                    }
                    else
                    {
                        finderpattern[i, j] = new Module(false,true,true);
                    }
                }
            }
            ArrayOps.Insert(this.matrix,finderpattern,new int[]{0,0});
            ArrayOps.Insert(this.matrix,finderpattern,new int[]{length-7,0});
            ArrayOps.Insert(this.matrix,finderpattern,new int[]{length-7,length-7});

            //add alignment patterns
            if (this.version == 2){
                Module[,] alignmentpatt = new Module[5, 5];
                
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (i == 0 || j == 0 || i == 4 || j == 4) alignmentpatt[i, j] = new Module(true, true, true);
                        else if (i == 2 && j == 2) alignmentpatt[i, j] = new Module(true, true, true);
                        else alignmentpatt[i, j] = new Module(false, true, true);
                    }
                }
                ArrayOps.Insert(this.matrix,alignmentpatt,new int[] {4,length-8});
            }
            
            //add separators
            for (int i = 0; i < 8; i++)
            {
                this.matrix[i, 7] = new Module(false, true, true);
                this.matrix[7, i] = new Module(false, true, true);
                this.matrix[length-7-1,i] = new Module(false, true, true);
                this.matrix[length-i-1,7] = new Module(false, true, true);
                this.matrix[length-i-1,length-7-1] = new Module(false, true, true);
                this.matrix[length-1-7,length-i-1] = new Module(false, true, true);
            }

            //add timing patterns
            for (int i = 0; i < length-16; i++)
            {
                this.matrix[8+i, 5] = new Module(Convert.ToBoolean(1-i % 2), true, true);
                this.matrix[length-1-5,8+i] = new Module(Convert.ToBoolean(1-i % 2), true, true);
            }
            
            //add dark module
            this.matrix[7, 8] = new Module(false, true, true);

        }
        
        //generate the image
        public void Generate()
        {
            var imagebytes = MyImage.ToByteArray(this.matrix);
            var header = MyImage.HeaderBuilder(this.matrix);
            var allbytes = MyImage.ArrAppend(header, imagebytes);
            File.WriteAllBytes("QRCode.bmp",allbytes);
        }

        public static string Encode(string data)
        {
            var alpha = File.ReadAllLines(@"C:\Users\racel\RiderProjects\qrcode\QR code generator\bin\Debug\alpha_table.txt");
            string[][] table = new string[alpha.Length][];
            
            for (int i = 0; i < alpha.Length; i++)
            {
                string number = i.ToString();
                var arr = new string[2] {Convert.ToString(alpha[i][0]),i.ToString()};
                table[i] = arr;
            }
            
            string binstr = "";
            for (int i = 0; i < data.Length/2*2; i += 2)
            {
                string a = Convert.ToString(data[i]);
                string b = Convert.ToString(data[i + 1]);
                int c1 = 0, c2 = 0;
                
                for (int j = 0; j < table.GetLength(0); j++)
                {
                    if (a == table[j][0])
                    {
                        c1 = Convert.ToInt32(table[j][1]);
                    }
                    if (b == table[j][0])
                    {
                        c2 = Convert.ToInt32(table[j][1]);
                    }
                }
                int tot = 45*c1 + c2;
                string addbin = Convert.ToString(tot, 2).PadLeft(11, '0');
                binstr += addbin;
            }
            
            if (data.Length % 2 == 1)
            {
                var c = Convert.ToString(data[data.Length - 1]);
                int res = 0;
                for (int j = 0; j < table.GetLength(0); j++)
                {
                    if (c == table[j][0])
                    {
                        res = Convert.ToInt32(table[j][1]);
                        break;
                    }
                }
                binstr += Convert.ToString(res,2).PadLeft(6,'0');
            }
            
            return binstr;
        }

        private static byte[] ErrorCorrection(string bindata)
        {
            byte[] arr = ArrayOps.BinStrToBytes(bindata);
            var corrbytes = ReedSolomonAlgorithm.Encode(arr,7,ErrorCorrectionCodeType.QRCode);
            return corrbytes;
        }
    }

    internal class Module : Pixel
    {
        public readonly bool value;
        public readonly bool skip;
        public readonly bool reserved;

        public Module(bool p, bool skip = false, bool res = false)
        {
            this.value = p;
            this.reserved = res;
            try
            {
                this.Red = Convert.ToByte(255 - Convert.ToByte(p) * 255);
                this.Green = Convert.ToByte(255 - Convert.ToByte(p) * 255);
                this.Blue = Convert.ToByte(255 - Convert.ToByte(p) * 255);
                this.skip = skip;
            }

            //should never happen
            catch (OverflowException)
            {
                Console.WriteLine("Pixel color byte conversion resulted in a number over 255");
            }
        }

        /*public Module(int p)
        {
            this.value = Convert.ToBoolean(p);
            try
            {
                this.Red = Convert.ToByte(255 - Convert.ToByte(p) * 255);
                this.Green = Convert.ToByte(255 - Convert.ToByte(p) * 255);
                this.Blue = Convert.ToByte(255 - Convert.ToByte(p) * 255);
            }

            //should never happen
            catch (OverflowException)
            {
                Console.WriteLine("Pixel color byte conversion resulted in a number over 255");
            }
        }*/
        public Module(byte p)
        {
            this.value = Convert.ToBoolean(p);
            try
            {
                this.Red = Convert.ToByte(255 - p * 255);
                this.Green = Convert.ToByte(255 - p * 255);
                this.Blue = Convert.ToByte(255 - p * 255);
            }

            //should never happen
            catch (OverflowException)
            {
                Console.WriteLine("Pixel color byte conversion resulted in a number over 255");
            }
        }
    }
}

