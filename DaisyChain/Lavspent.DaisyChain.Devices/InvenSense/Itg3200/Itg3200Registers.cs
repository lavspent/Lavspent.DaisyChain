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

namespace Lavspent.DaisyChain.Devices.InvenSense.Itg3200
{
    /// <summary>
    /// Itg3200 Registers
    /// </summary>
    public class Itg3200Registers
    {
        //ADDRESS = 0x68
        public const byte WHO = 0x00;
        public const byte SMPL = 0x15;
        public const byte DLPF = 0x16;
        public const byte INT_C = 0x17;
        public const byte INT_S = 0x1A;
        public const byte TMP_H = 0x1B;
        public const byte TMP_L = 0x1C;
        public const byte GX_H = 0x1D;
        public const byte GX_L = 0x1E;
        public const byte GY_H = 0x1F;
        public const byte GY_L = 0x20;
        public const byte GZ_H = 0x21;
        public const byte GZ_L = 0x22;
        public const byte PWR_M = 0x3E;
    }
}

