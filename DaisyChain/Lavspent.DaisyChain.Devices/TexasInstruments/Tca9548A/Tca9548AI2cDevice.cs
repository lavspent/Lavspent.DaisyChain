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

using Lavspent.DaisyChain.I2c;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Devices.TexasInstruments.Tca9548A
{

    /// <summary>
    /// 
    /// </summary>
    // TODO: Move somewhere
    internal class Tca9548AI2cDevice : II2cDevice
    {
        private Tca9548A _tca9548A;
        private II2cDevice _i2cDevice;
        private byte _output;


        //TODO: Clean up this argument list
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tca9548A"></param>
        /// <param name="i2cDevice"></param>
        /// <param name="output"></param>
        /// <param name="connectionSettings"></param>
        /// <param name="deviceId"></param>
        public Tca9548AI2cDevice(Tca9548A tca9548A, II2cDevice i2cDevice, byte output, I2cConnectionSettings connectionSettings, string deviceId)
        {
            _tca9548A = tca9548A;
            _i2cDevice = i2cDevice;
            ConnectionSettings = connectionSettings;
            _output = output;
            DeviceId = deviceId;
        }

        /// <summary>
        /// 
        /// </summary>
        public I2cConnectionSettings ConnectionSettings
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string DeviceId
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task ReadAsync(byte[] buffer, CancellationToken cancellationToken)
        {
            return _i2cDevice.ReadAsync(buffer, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task<I2cTransferResult> ReadPartialAsync(byte[] buffer, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public async Task WriteAsync(byte[] buffer, CancellationToken cancellationToken)
        {
            // Select the right output
            await _tca9548A.SelectAsync(_output, cancellationToken).ConfigureAwait(false);

            // Write to "child" device
            await _i2cDevice.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task<I2cTransferResult> WritePartialAsync(byte[] buffer, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writeBuffer"></param>
        /// <param name="readBuffer"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public async Task WriteReadAsync(byte[] writeBuffer, byte[] readBuffer, CancellationToken cancellationToken)
        {
            // Select the right output
            await _tca9548A.SelectAsync(_output, cancellationToken).ConfigureAwait(false);
            // Write data
            await _i2cDevice.WriteAsync(writeBuffer, cancellationToken).ConfigureAwait(false);
            // Read data
            await _i2cDevice.ReadAsync(readBuffer, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writeBuffer"></param>
        /// <param name="readBuffer"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task<I2cTransferResult> WriteReadPartialAsync(byte[] writeBuffer, byte[] readBuffer, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
