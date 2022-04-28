using System;

namespace QR_code_generator
{
    public static class ArrayOps
    {
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
        public static void Fuse<T>(T[] arr1,T[] arr2,int index = 0)
        {
            try
            {
                for (int i = index; i < arr2.Length + index; i++)
                {
                    arr1[i] = arr2[i];
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }
        }
        public static void Insert<T>(T[,] mat1, T[,] mat2, int[] coord)
        {
            for (int i = 0; i < mat2.GetLength(0); i++)
            {
                for (int j = 0; j < mat2.GetLength(1); j++)
                {
                    mat1[i + coord[0], j + coord[1]] = mat2[i, j];
                }
            }
        }
        public static byte[] BinStrToBytes(string bits)
        {
            if (bits.Length % 8 != 0)
            {
                throw new ArgumentException("Bits string has a length that is not a multiple of 8");
            }

            byte[] res = new byte[bits.Length / 8];
            
            for (int i = 0; i < bits.Length; i += 8)
            {
                string bytestr = "";
                for (int j = 0; j < 8; j++)
                {
                    bytestr += bits[i + j];
                }
                res[i / 8] = Convert.ToByte(bytestr, 2);
            }
            return res;
        }
        public static string BytesToBinStr(byte[] arr)
        {
            string res = "";
            for (int i = 0; i < arr.Length; i++)
            {
                res += Convert.ToString(arr[i], 2).PadLeft(8, '0');
            }
            return res;
        }
        public static bool[] BytesToBoolArr(byte[] arr) => BinStringToBoolArr(BytesToBinStr(arr));
        public static byte[] BoolArrToBytes(bool[] arr) => BinStrToBytes(BoolArrToBinString(arr));

    }
}