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

using Lavspent.DaisyChain.Stream;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Serial
{
    /// <summary>
    /// Interface to a serial device (UART, software serial etc.)
    /// </summary>
    public interface ISerial : IDisposable
    {
        /// <summary>
        /// Get or sets the number of data bits used by this serial device.
        /// </summary>
        ushort DataBits { get; set; }

        /// <summary>
        /// Gets og sets the number of stop bits used bu this serial device.
        /// </summary>
        StopBits StopBits { get; set; }

        /// <summary>
        /// Gets or sets the parity used by this serial device.
        /// </summary>
        Parity Parity { get; set; }

        /// <summary>
        /// Gets or sets the Handshake use by this serial device.
        /// </summary>
        Handshake Handshake { get; set; }

        /// <summary>
        /// Get the stream of this serial device.
        /// </summary>
        /// <returns></returns>
        IStream GetStream();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task OpenAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task CloseAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
