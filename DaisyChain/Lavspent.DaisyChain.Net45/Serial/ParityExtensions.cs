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
    /// Extension methods for .Net System.IO.Ports.Parity.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ParityExtensions
    {
        /// <summary>
        /// Converts this .Net Parity to a DaisyChain Parity.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static Parity AsParity(this System.IO.Ports.Parity _this)
        {
            switch (_this)
            {
                case System.IO.Ports.Parity.None:
                    return Parity.None;
                case System.IO.Ports.Parity.Odd:
                    return Parity.Odd;
                case System.IO.Ports.Parity.Even:
                    return Parity.Even;
                case System.IO.Ports.Parity.Mark:
                    return Parity.Mark;
                case System.IO.Ports.Parity.Space:
                    return Parity.Space;
                default:
                    throw new Exception($"Unexpected Parity {_this}.");
            }
        }

        /// <summary>
        /// Convert this DaisyChain Parity as a .Net Parity.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static System.IO.Ports.Parity AsNativeParity(this Parity _this)
        {
            switch (_this)
            {
                case Parity.None:
                    return System.IO.Ports.Parity.None;
                case Parity.Odd:
                    return System.IO.Ports.Parity.Odd;
                case Parity.Even:
                    return System.IO.Ports.Parity.Even;
                case Parity.Mark:
                    return System.IO.Ports.Parity.Mark;
                case Parity.Space:
                    return System.IO.Ports.Parity.Space;
                default:
                    throw new Exception($"Unexpected Parity {_this}.");
            }
        }
    }
}
