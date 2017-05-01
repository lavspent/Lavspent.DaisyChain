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

namespace Lavspent.DaisyChain.Firmata
{
    internal class FirmataI2cDevice : II2cDevice
    {
        private FirmataClient _firmataClient;
        private FirmataI2cController _i2cController;

        public FirmataI2cDevice(FirmataClient firmataClient, FirmataI2cController i2cController, I2cConnectionSettings connectionSettings)
        {
            _firmataClient = firmataClient;
            ConnectionSettings = connectionSettings;
            _i2cController = i2cController;
        }

        public I2cConnectionSettings ConnectionSettings
        {
            get;
            private set;
        }

        // TODO: Implement
        public string DeviceId
        {
            get
            {
                return _i2cController.DeviceId;
            }
        }

        public async Task ReadAsync(byte[] buffer, CancellationToken cancellationToken)
        {
            // send a read request
            var result = await _firmataClient.SendI2cReadAsync(
                ConnectionSettings.SlaveAddress, (byte)buffer.Length, cancellationToken
                ).ConfigureAwait(false);

            // todo: What if one provide a buffer longer than the response?
            // should the task return actual number of bytes?
            Array.Copy(result.I2cData, buffer, buffer.Length);

            return;
        }

        // TODO: Implement
        public Task<I2cTransferResult> ReadPartialAsync(byte[] buffer, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task WriteAsync(byte[] buffer, CancellationToken cancellationToken)
        {
            return _firmataClient.SendI2cWriteRequestAsync(ConnectionSettings.SlaveAddress, buffer, cancellationToken);
        }

        public async Task<I2cTransferResult> WritePartialAsync(byte[] buffer, CancellationToken cancellationToken)
        {
            await WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
            return new I2cTransferResult()
            {
                BytesTransferred = (uint)buffer.Length,
                Status = I2cTransferStatus.FullTransfer
            };
        }

        public async Task WriteReadAsync(byte[] writeBuffer, byte[] readBuffer, CancellationToken cancellationToken)
        {
            // Todo: Is is more "reliable" to fire off a thread here?
            await WriteAsync(writeBuffer, cancellationToken).ConfigureAwait(false);
            await ReadAsync(readBuffer, cancellationToken).ConfigureAwait(false);
        }

        // TODO: Implement
        public Task<I2cTransferResult> WriteReadPartialAsync(byte[] writeBuffer, byte[] readBuffer, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
