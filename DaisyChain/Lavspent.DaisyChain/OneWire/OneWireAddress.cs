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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.OneWire
{
    public class OneWireAddress
    {
        private byte[] _raw;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        public OneWireAddress(byte[] address)
        {
            if (!IsValid(address))
                throw new ArgumentException("Address is not valid.", nameof(address));

            this._raw = new byte[8];
            address.CopyTo(_raw, 0);
        }

        /// <summary>
        /// Convert this array of bytes to a OneWireAddress.
        /// </summary>
        /// <param name="address"></param>
        public static implicit operator OneWireAddress(byte[] address)
        {
            return new OneWireAddress(address);
        }

        /// <summary>
        /// Convert this OneWireAddress to a byte array.
        /// </summary>
        /// <param name="address"></param>
        public static implicit operator byte[] (OneWireAddress address)
        {
            return address.Raw;
        }

        /// <summary>
        /// Convert this OneWireAddress to a byte array.
        /// </summary>
        /// <param name="address"></param>
        public static implicit operator OneWireAddress(string hex)
        {
            return HexStringToByteArray(hex);
        }

        /// <summary>
        /// 
        /// </summary>
        public byte[] Raw
        {
            get
            {
                byte[] addressCopy = new byte[8];
                _raw.CopyTo(addressCopy, 0);
                return addressCopy;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte[] SerialNumber
        {
            get
            {
                byte[] serialNumber = new byte[6];
                Array.Copy(_raw, 1, serialNumber, 0, 6);
                return serialNumber;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte FamilyCode
        {
            get
            {
                return _raw[0];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte Crc
        {
            get
            {
                return _raw[7];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsValid(byte[] address)
        {
            return
                address != null &&
                address.Length == 8 &&
                CalculateCrc(address, 0, 7) == address[7];
        }

        static readonly byte[] crcLookupTable = new byte[]
        {
            0, 94, 188, 226, 97, 63, 221, 131, 194, 156, 126, 32, 163, 253, 31, 65,
            157, 195, 33, 127, 252, 162, 64, 30, 95, 1, 227, 189, 62, 96, 130, 220,
            35, 125, 159, 193, 66, 28, 254, 160, 225, 191, 93, 3, 128, 222, 60, 98,
            190, 224, 2, 92, 223, 129, 99, 61, 124, 34, 192, 158, 29, 67, 161, 255,
            70, 24, 250, 164, 39, 121, 155, 197, 132, 218, 56, 102, 229, 187, 89, 7,
            219, 133, 103, 57, 186, 228, 6, 88, 25, 71, 165, 251, 120, 38, 196, 154,
            101, 59, 217, 135, 4, 90, 184, 230, 167, 249, 27, 69, 198, 152, 122, 36,
            248, 166, 68, 26, 153, 199, 37, 123, 58, 100, 134, 216, 91, 5, 231, 185,
            140, 210, 48, 110, 237, 179, 81, 15, 78, 16, 242, 172, 47, 113, 147, 205,
            17, 79, 173, 243, 112, 46, 204, 146, 211, 141, 111, 49, 178, 236, 14, 80,
            175, 241, 19, 77, 206, 144, 114, 44, 109, 51, 209, 143, 12, 82, 176, 238,
            50, 108, 142, 208, 83, 13, 239, 177, 240, 174, 76, 18, 145, 207, 45, 115,
            202, 148, 118, 40, 171, 245, 23, 73, 8, 86, 180, 234, 105, 55, 213, 139,
            87, 9, 235, 181, 54, 104, 138, 212, 149, 203, 41, 119, 244, 170, 72, 22,
            233, 183, 85, 11, 136, 214, 52, 106, 43, 117, 151, 201, 74, 20, 246, 168,
            116, 42, 200, 150, 21, 75, 169, 247, 182, 232, 10, 84, 215, 137, 107, 53
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte CalculateCrc(byte[] data, int startIndex, int length)
        {
            byte crc = 0;
            for (int i = startIndex; i < length; i++)
            {
                crc = crcLookupTable[crc ^ data[i]];
            }

            return crc;
        }

        /// <summary>
        /// Converte a hexadecimal formatted string to an array of bytes.
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        // TODO: Some magic that propbably belongs somewhere else
        private static byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
