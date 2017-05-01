/*
    Copyright(c) 2017 Petter Labråten/LAVSPENT.NO. All rights reserved.

    The MIT License(MIT)

    Permission is hereby granted, free of charge, to any person obtaining a
    copy of this software and associated documentation files (the "Software"),
    to deal in the Software without restriction, including without limitation
    the rights to use, copy, modify, merge, publish, distribute, sublicense,
    and/or sell copies of the Software, and to permit persons to whom the
    Software is furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
    DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Text;

namespace Lavspent.DaisyChain.Firmata
{
    public static class FirmataUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] MidiEncodeUShort(ushort value)
        {
            if (value > 0x3fff)
                throw new ArgumentOutOfRangeException(nameof(value));

            byte l = (byte)(value & 0x007f);
            byte h = (byte)((value >> 7) & 0x007f);

            return new byte[] { l, h };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static ushort MidiDecodeUShort(byte[] data, int offset = 0)
        {
            if (data == null || data.Length - offset < 2)
                throw new ArgumentException("Must be two bytes long.", nameof(data));

            return (ushort)((data[offset] & 0x007f) | ((data[offset + 1] & 0x007f) << 7));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] Encode7BitString(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            return Encode7BitData(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] Encode7BitData(byte[] data, int offset, int length)
        {
            int resultLength = length * 2;
            byte[] result = new byte[resultLength];
            for (int i = 0; i < length; i++)
            {
                result[i * 2] = (byte)(data[offset + i] & 0x7f);
                result[i * 2 + 1] = (byte)((data[offset + i] >> 7) & 0x7f);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Decode7BitString(byte[] data, int offset, int length)
        {
            try
            {
                var bytes = Decode7BitData(data, offset, length);
                return new String(Encoding.UTF8.GetChars(bytes));
            }
            catch (Exception e)
            {
                throw new Exception("Failed to decode string.", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] Decode7BitData(byte[] data, int offset, int length)
        {
            if (length % 2 != 0)
                throw new ArgumentException("Must be a multiple of 2.", nameof(length));

            int resultLength = length / 2;
            byte[] result = new byte[resultLength];
            for (int i = 0; i < resultLength; i++)
            {
                result[i] = (byte)(data[i * 2 + offset] | data[i * 2 + offset + 1] << 7);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Decode7BitData(byte[] data)
        {
            return Decode7BitData(data, 0, data.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static int GetCommandLength(uint command)
        {
            int i = 1;
            while (command > 0xff)
            {
                command >>= 8;
                i++;
            }
            return i;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static byte[] CommendToBytes(uint command)
        {
            byte[] bytes = new byte[GetCommandLength(command)];
            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                bytes[i] = (byte)(command & 0xff);
                command >>= 8;
            }
            return bytes;

        }
    }
}
