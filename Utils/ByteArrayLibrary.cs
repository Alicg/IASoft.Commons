using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Utils
{
    public static class ByteArrayLibrary
    {
        public static byte[] JoinArrays(byte[] b1, byte[] b2)
        {
            var ba = new byte[b1.Length + b2.Length];
            Buffer.BlockCopy(b1, 0, ba, 0, b1.Length);
            Buffer.BlockCopy(b2, 0, ba, b1.Length, b2.Length);
            return ba;
        }

        public static byte[] JoinArrays(List<byte[]> byteArrays)
        {
            if (byteArrays == null)
                throw new ArgumentNullException("byteArrays");

            using (var ms = new MemoryStream())
            {
                foreach (var ba in byteArrays)
                {
                    if (ba == null)
                        continue;
                    ms.Write(ba, 0, ba.Length);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Extension. Пытается добавить в начало массива любое значение. Если это простейший тип - добавляет, иначе возвращает неизмененный массив.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="value">может быть bool,char,char[],decimal,double,float,int,long,object,string,uint,ulong</param>
        /// <returns></returns>
        public static byte[] AddToArrayBeginning(this byte[] array, dynamic value)
        {
            try
            {
                dynamic arr = BitConverter.GetBytes(value);
                return JoinArrays(arr, array);
            }
            catch
            {
                return array;
            }
        }

        /// <summary>
        /// Extension. Пытается добавить в конец массива любое значение. Если это простейший тип - добавляет, иначе возвращает неизмененный массив.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="value">может быть bool,char,char[],decimal,double,float,int,long,object,string,uint,ulong</param>
        /// <returns></returns>
        public static byte[] AddToArrayEnding(this byte[] array, dynamic value)
        {
            try
            {
                using (var ms = new MemoryStream(array))
                {
                    using (var sw = new StreamWriter(ms))
                    {
                        sw.Write(value);
                        sw.Flush();
                    }
                }
                return array;
            }
            catch
            {
                return array;
            }
        }

        /// <summary>
        /// Сравнивает два массива байт побайтно
        /// </summary>
        public static bool ArraysEqual(byte[] ba1, byte[] ba2)
        {
            if (ba1 == null)
                throw new ArgumentNullException("ba1");
            if (ba2 == null)
                throw new ArgumentNullException("ba2");

            if (ba1.Length != ba2.Length)
                return false;

            return !ba1.Where((t, i) => t != ba2[i]).Any();
        }

        public static byte[] BitArrayToByteArray(BitArray bits)
        {
            var numBytes = bits.Count / 8;
            if (bits.Count % 8 != 0) numBytes++;

            var bytes = new byte[numBytes];
            int byteIndex = 0, bitIndex = 0;

            for (int i = 0; i < bits.Count; i++)
            {
                if (bits[i])
                    bytes[byteIndex] |= (byte)(1 << (7 - bitIndex));

                bitIndex++;
                if (bitIndex == 8)
                {
                    bitIndex = 0;
                    byteIndex++;
                }
            }

            return bytes;
        }
    }
}