using System;
using Projet_Info;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Xml.Serialization;

namespace QR_code_generator
{
    internal class Program
    {
        public static void Main()
        {
            
        }
    }

    public class QRCode : MyImage
    {
        private byte version;
        private string binstr;
        private string data;
        private bool[] mode = {false, false, true, false};
        private char correction_mode = 'L';
        private bool[] length_bits;
        private bool[] encoded_data;
        private bool[] correctionbits;
        private bool[] binaries;
        private bool[] allbytes;
        
        public QRCode(string text, char corr_mode = 'L')
        {
            binstr = BoolArrToBinString(mode)+Convert.ToString(text.Length,2).PadLeft(9,'0');
            this.data = text;
            this.correction_mode = corr_mode;
            this.encoded_data = BinStringToBoolArr(Encode(text));
            binstr += BoolArrToBinString(this.encoded_data);
            while (binstr.Length % 8 != 0)
            {
                binstr += '0';
            }
            
        }

        //generate the image
        public void Generate()
        {

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

        public static string BoolArrToBinString(bool[] arr)
        {
            string res = "";
            foreach (var v in arr)
            {
                if (v) res += "1";
                else res += "0";
            }

            return res;
        }

        public static bool[] BinStringToBoolArr(string s)
        {
            bool[] res = new bool[s.Length];
            for (var index = 0; index < s.Length; index++)
            {
                var c = s[index];
                if (c == '1') res[index] = true;
                else res[index] = false;
            }
            return res;
        }
    }

    internal class Module : Pixel
    {
        private readonly bool value;
        public readonly bool skip;

        public Module(bool p, bool skip)
        {
            this.value = p;
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

        public Module(int p)
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
        }

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

