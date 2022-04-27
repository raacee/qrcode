using System;
using Projet_Info;
using System.Diagnostics;
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
            binstr = "";
            this.data = text;
            this.correction_mode = corr_mode;
            binstr += BoolArrToBinString(this.mode) + Convert.ToString(text.Length, 2).PadLeft(9, '0');
        }

        //generate the image
        public void Generate()
        {

        }

        public static string Encode(string data)
        {
            string binstr = "";
            for (int i = 0; i < data.Length / 2; i += 2)
            {
                int a = data[i];
                int b = data[i + 1];
                a *= 45;
                int tot = a + b;
                binstr += Convert.ToString(tot, 2).PadLeft(11, '0');
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

