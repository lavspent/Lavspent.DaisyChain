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
using System.ComponentModel;
using Windows.Devices.SerialCommunication;

namespace Lavspent.DaisyChain.Serial
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ParityExtensions
    {
        /// <summary>
        /// Convert this UWP SerialParity to a DaisyChain Parity.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static Parity AsParity(this SerialParity _this)
        {
            switch (_this)
            {
                case SerialParity.None:
                    return Parity.None;
                case SerialParity.Odd:
                    return Parity.Odd;
                case SerialParity.Even:
                    return Parity.Even;
                case SerialParity.Mark:
                    return Parity.Mark;
                case SerialParity.Space:
                    return Parity.Space;
                default:
                    throw new Exception($"Unknown SerialParity type: {_this}");

            }
        }

        /// <summary>
        /// Convert this DaisyChain Parity to a UWP SerialParity.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static SerialParity AsNativeSerialParity(this Parity _this)
        {
            switch (_this)
            {
                case Parity.None:
                    return SerialParity.None;
                case Parity.Odd:
                    return SerialParity.Odd;
                case Parity.Even:
                    return SerialParity.Even;
                case Parity.Mark:
                    return SerialParity.Mark;
                case Parity.Space:
                    return SerialParity.Space;
                default:
                    throw new Exception($"Unknown Parity type: {_this}");
            }
        }
    }
}
