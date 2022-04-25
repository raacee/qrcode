using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace QR_code_generator
{
    internal class Program
    {
        public static void Main()
        {
            QRCode test = new QRCode("Hello World");
            test.Generate();
        }
    }

    public class QRCode
    {
        private byte version = 1;
        private bool[] mode = {false,false,true,false};
        private bool[] length;
        private bool[] information;
        private bool[] data;
        private bool[] allbytes;
        
        public QRCode(string text)
        {
            
        }
        
        //generate the image
        public void Generate()
        {
            
        }

        public static string BoolArrayToBinString(bool[] arr)
        {
            string res = "";
            foreach (var v in arr)
            {
                if (v) res += "1";
                else res += "0";
            }
            return res;
        }
    }

    internal class Module
    { 
        public readonly bool value;
        public readonly bool skippable;
        public readonly byte red;
        public readonly byte green;
        public readonly byte blue;
        public Module(bool p)
        {
            this.value = p;
            try
            {
                this.red = Convert.ToByte(255-Convert.ToByte(p) * 255);
                this.green = Convert.ToByte(255-Convert.ToByte(p) * 255);
                this.blue = Convert.ToByte(255-Convert.ToByte(p) * 255);
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
                this.red = Convert.ToByte(255-Convert.ToByte(p) * 255);
                this.green = Convert.ToByte(255-Convert.ToByte(p) * 255);
                this.blue = Convert.ToByte(255-Convert.ToByte(p) * 255);
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
                this.red = Convert.ToByte(255-p * 255);
                this.green = Convert.ToByte(255-p * 255);
                this.blue = Convert.ToByte(255-p * 255);
            }
            
            //should never happen
            catch (OverflowException)
            {
                Console.WriteLine("Pixel color byte conversion resulted in a number over 255");
            }
        }
    }
    public class ColoredException : Exception
    {
        private new string Message = "Module is colored, check pixels value";
    }
    
    
}