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
    /// Wraps a native Stream as a DaisyChain IStream.
    /// </summary>
    internal class PclStreamWrapper : IStream
    {
        private System.IO.Stream _stream;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public PclStreamWrapper(System.IO.Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Asynchronously reads a byte from the underlying stream.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public async Task<int> ReadAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            byte[] singleByteBuffer = new byte[1];
            int read = await ReadAsync(singleByteBuffer, 0, 1, cancellationToken).ConfigureAwait(false);

            if (read == 0)
                return -1;

            return singleByteBuffer[0];
        }

        /// <summary>
        /// Asynchronously reads a chunck of bytes from the underlying stream.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public async Task<int> ReadAsync(byte[] buffer, int index, int length, CancellationToken cancellationToken = default(CancellationToken))
        {
            CancellationTokenSource cts = new CancellationTokenSource(_stream.ReadTimeout);
            CancellationTokenSource ctsl = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);
            int read = 0;
            try
            {
                read = await _stream.ReadAsync(buffer, index, length, ctsl.Token /* cancellationToken*/);
            } catch (TaskCanceledException tce) when (ctsl.IsCancellationRequested)
            {
                // timeout
                read = 0;
            }
            return read;
        }

        /// <summary>
        /// Asynchronously writes a byte to the underlying stream.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task WriteAsync(byte data, CancellationToken cancellationToken)
        {
            // TODO: Cache singlebytebuffer
            byte[] singleByteBuffer = new byte[] { data };
            return WriteAsync(singleByteBuffer, 0, 1, cancellationToken);
        }

        /// <summary>
        /// Asynchronously writes data to the underlying stream.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        /// 
        public async Task WriteAsync(byte[] data, int index, int length, CancellationToken cancellationToken)
        {
            await _stream.WriteAsync(data, index, length, cancellationToken).ConfigureAwait(false);
            await _stream.FlushAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task FlushAsync(CancellationToken cancellationToken)
        {
            return _stream.FlushAsync(cancellationToken);
        }


        public TimeSpan ReadTimeout
        {
            get
            {
                return TimeSpan.FromMilliseconds(_stream.ReadTimeout);
            }

            set
            {
                _stream.ReadTimeout = (int)value.TotalMilliseconds;
            }
        }

        public TimeSpan WriteTimeout
        {
            get
            {
                return TimeSpan.FromMilliseconds(_stream.WriteTimeout);
            }

            set
            {
                _stream.WriteTimeout = (int)value.TotalMilliseconds;
            }
        }

    }

}
