using System;
using System.Runtime.CompilerServices;
using Projet_Info;

namespace QR_code_generator
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            
        }
    }

    public class Module : Pixel
    {
        private bool value;
        public Module(bool p)
        {
            this.value = p;
            try
            {
                this.Red = Convert.ToByte(Convert.ToByte(p) * 255);
                this.Green = Convert.ToByte(Convert.ToByte(p) * 255);
                this.Blue = Convert.ToByte(Convert.ToByte(p) * 255);
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