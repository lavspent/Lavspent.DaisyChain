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
    public static class StopBitsExtensions
    {
        /// <summary>
        /// Convert this UWP SerialStopBitCount to a DaisyChain StopBits enumerable.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static StopBits AsStopBits(this SerialStopBitCount _this)
        {
            switch (_this)
            {
                case SerialStopBitCount.One:
                    return StopBits.One;
                case SerialStopBitCount.OnePointFive:
                    return StopBits.OnePointFive;
                case SerialStopBitCount.Two:
                    return StopBits.Two;
                default:
                    throw new Exception($"Unexpected SerialStopBitCount {_this}.");
            }
        }

        /// <summary>
        /// Converts this DaisyChain StopBits to a UWP SerialStopBitCount.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static SerialStopBitCount AsNativeSerialStopBitCount(this StopBits _this)
        {
            switch (_this)
            {
                case StopBits.One:
                    return SerialStopBitCount.One;
                case StopBits.OnePointFive:
                    return SerialStopBitCount.OnePointFive;
                case StopBits.Two:
                    return SerialStopBitCount.Two;
                case StopBits.None:
                    throw new Exception($"StopBits.None cannot be converted to a SerialStopBitCount.");
                default:
                    throw new Exception($"Unexpected StopBits {_this}.");
            }
        }
    }
}
