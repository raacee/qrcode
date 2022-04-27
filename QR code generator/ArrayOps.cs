﻿using System;

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
        
        public static void Fuse<T>(T[] arr1,T[] arr2)
        {
            if (arr2.Length > arr1.Length) throw new ArgumentException("Second argument is bigger than first");

            for (int i = 0; i < arr1.Length; i++)
            {
                arr1[i] = arr2[i];
            }
        }
        
        
    }
}