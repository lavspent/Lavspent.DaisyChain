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
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Lavspent.DaisyChain.Stream
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AbstractUwpStorageStreamWrapper : IStream
    {
        private IInputStream _inputStream;
        private IOutputStream _outputStream;

        public abstract TimeSpan ReadTimeout { get; set; }

        public abstract TimeSpan WriteTimeout { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="outputStream"></param>
        public AbstractUwpStorageStreamWrapper(IInputStream inputStream, IOutputStream outputStream)
        {
            _inputStream = inputStream;
            _outputStream = outputStream;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public async Task<int> ReadAsync(CancellationToken cancellationToken)
        {
            IBuffer buffer = await _inputStream.ReadAsync(
                new Windows.Storage.Streams.Buffer(1),
                1,
                InputStreamOptions.None
                ).AsTask(cancellationToken);

            return buffer.ToArray()[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public async Task<int> ReadAsync(byte[] buffer, int index, int length, CancellationToken cancellationToken)
        {
            // TODO: Clean up names here. buffer buffer, buffer...
            IBuffer abuffer = new Windows.Storage.Streams.Buffer((uint)length);

            IBuffer resultBuffer =
                await _inputStream.ReadAsync(
                    abuffer,
                    (uint)length,
                    InputStreamOptions.None
                    ).AsTask(cancellationToken);

            resultBuffer.CopyTo(0, buffer, index, length);
            return (int)resultBuffer.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task WriteAsync(byte data, CancellationToken cancellationToken)
        {
            byte[] singleByteBuffer = new byte[1];
            singleByteBuffer[0] = data;

            return WriteAsync(singleByteBuffer, 0, 1, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task WriteAsync(byte[] data, int index, int length, CancellationToken cancellationToken)
        {
            IBuffer writeBuffer = data.AsBuffer(index, length);
            return _outputStream.WriteAsync(writeBuffer).AsTask(cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task FlushAsync(CancellationToken cancellationToken)
        {
            return _outputStream.FlushAsync().AsTask(cancellationToken);
        }
    }
}
