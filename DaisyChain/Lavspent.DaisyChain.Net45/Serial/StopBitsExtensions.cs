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

namespace Lavspent.DaisyChain.Serial
{
    /// <summary>
    /// Extension methods for .Net System.IO.Ports.StopBits.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class StopBitsExtensions
    {
        /// <summary>
        /// Convert this .Net45 StopBits to a DaisyChain StopBits enumerable.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static StopBits AsStopBits(this System.IO.Ports.StopBits _this)
        {
            switch (_this)
            {
                case System.IO.Ports.StopBits.One:
                    return StopBits.One;
                case System.IO.Ports.StopBits.OnePointFive:
                    return StopBits.OnePointFive;
                case System.IO.Ports.StopBits.Two:
                    return StopBits.Two;
                default:
                    throw new Exception($"Unexpected StopBits {_this}.");
            }
        }

        /// <summary>
        /// Converts this DaisyChain StopBits to a .Net45 SerialStopBitCount.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static System.IO.Ports.StopBits AsNativeStopBits(this StopBits _this)
        {
            switch (_this)
            {
                case StopBits.One:
                    return System.IO.Ports.StopBits.One;
                case StopBits.OnePointFive:
                    return System.IO.Ports.StopBits.OnePointFive;
                case StopBits.Two:
                    return System.IO.Ports.StopBits.Two;
                case StopBits.None:
                    return System.IO.Ports.StopBits.None;
                default:
                    throw new Exception($"Unexpected StopBits {_this}.");
            }
        }
    }
}
