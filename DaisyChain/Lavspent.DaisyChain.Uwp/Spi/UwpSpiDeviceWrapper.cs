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

using System.Threading.Tasks;
using Windows.Devices.Spi;

namespace Lavspent.DaisyChain.Spi
{
    internal class UwpSpiDeviceWrapper : ISpiChannel
    {
        private ISpiController _spiController;
        private SpiDevice _spiDevice;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spiDevice"></param>
        public UwpSpiDeviceWrapper(ISpiController spiController, SpiDevice spiDevice)
        {
            _spiController = spiController;
            _spiDevice = spiDevice;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public Task ReadAsync(byte[] buffer)
        {
            return Task.Run(() =>
            {
                _spiDevice.Read(buffer);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writeBuffer"></param>
        /// <param name="readBuffer"></param>
        /// <returns></returns>
        public Task TransferFullDuplexAsync(byte[] writeBuffer, byte[] readBuffer)
        {
            return Task.Run(() =>
            {
                _spiDevice.TransferFullDuplex(writeBuffer, readBuffer);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writeBuffer"></param>
        /// <param name="readBuffer"></param>
        /// <returns></returns>
        public Task TransferSequentialAsync(byte[] writeBuffer, byte[] readBuffer)
        {
            return Task.Run(() =>
            {
                _spiDevice.TransferSequential(writeBuffer, readBuffer);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public Task WriteAsync(byte[] buffer)
        {
            return Task.Run(() =>
            {
                _spiDevice.Write(buffer);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _spiDevice.Dispose();
        }
    }
}
