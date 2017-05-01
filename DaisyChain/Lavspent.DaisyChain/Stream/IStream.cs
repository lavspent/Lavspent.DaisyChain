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
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Stream
{
    /// <summary>
    /// 
    /// </summary>
    public interface IStream
    {
        /// <summary>
        /// Read an unsigned byte, cast as an int, from the stream.
        /// </summary>
        /// <returns>Byte read or -1 if stream has been closed.</returns>
        Task<int> ReadAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Read bytes from the stream.
        /// </summary>
        /// <returns>Number of bytes read or -1 if stream has been closed.</returns>
        Task<int> ReadAsync(byte[] buffer, int index, int length, CancellationToken cancellationToken = default(CancellationToken));

        //TODO: Maybe WriteAsync should return the amount of data written? Or?
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        Task WriteAsync(byte data, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        Task WriteAsync(byte[] data, int index, int length, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// 
        /// </summary>
        TimeSpan ReadTimeout { get; set; }

        /// <summary>
        /// 
        /// </summary>
        TimeSpan WriteTimeout { get; set; }
    }
}
