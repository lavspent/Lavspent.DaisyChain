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
using Lavspent.DaisyChain.OneWire;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Lavspent.DaisyChain.Devices.MaximIntegrated.Ds2482
{
    /// <summary>
    /// 
    /// </summary>
    public class Ds2482 : IOneWireBusController
    {
        private II2cDevice i2cDevice;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i2cDevice"></param>
        public Ds2482(II2cDevice i2cDevice)
        {
            this.i2cDevice = i2cDevice;
        }

        /// <summary>
        /// Resets the Ds2482
        /// </summary>
        /// <returns></returns>
        public Task DeviceResetAsync()
        {
            return WriteRawAsync(Ds2482CommandCommands.DeviceReset);
        }

        private byte[] readStatusWriteBuffer = new byte[] { Ds2482CommandCommands.SetReadPointer, (byte)Ds2482RegisterEnum.Status };
        private byte[] readStatusReadBuffer = new byte[1];

        /// <summary>
        /// Read the status register
        /// </summary>
        /// <returns></returns>
        public async Task<Ds2482StatusFlags> ReadStatusAsync()
        {
            await i2cDevice.WriteReadAsync(readStatusWriteBuffer, readStatusReadBuffer).ConfigureAwait(false);
            return (Ds2482StatusFlags)readStatusReadBuffer[0];
        }

        //
        private byte[] readDataWriteBuffer = new byte[] { Ds2482CommandCommands.SetReadPointer, (byte)Ds2482RegisterEnum.Data };
        private byte[] readDataReadBuffer = new byte[1];

        /// <summary>
        /// Read the data register
        /// </summary>
        /// <returns></returns>
        public async Task<byte> ReadDataAsync()
        {
            await i2cDevice.WriteReadAsync(readDataWriteBuffer, readDataReadBuffer).ConfigureAwait(false);
            return readDataReadBuffer[0];
        }

        //
        private byte[] readConfigurationWriteBuffer = new byte[] { Ds2482CommandCommands.SetReadPointer, (byte)Ds2482RegisterEnum.Configuration };
        private byte[] readConfigurationReadBuffer = new byte[1];

        /// <summary>
        /// Read the config register
        /// </summary>
        public async Task<Ds2482ConfigurationFlags> ReadConfigurationAsync()
        {
            await i2cDevice.WriteReadAsync(readConfigurationWriteBuffer, readConfigurationReadBuffer).ConfigureAwait(false);
            return (Ds2482ConfigurationFlags)readConfigurationReadBuffer[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task SetStrongPullupAsync()
        {
            return SetConfigurationFlagsAsync(Ds2482ConfigurationFlags.StrongPullUp, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task ClearStrongPullupAsync()
        {
            return SetConfigurationFlagsAsync(Ds2482ConfigurationFlags.StrongPullUp, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurationFlags"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task SetConfigurationFlagsAsync(Ds2482ConfigurationFlags configurationFlags, bool value)
        {
            Ds2482ConfigurationFlags newConfiguration = await ReadConfigurationAsync().ConfigureAwait(false);
            if (value)
            {
                // set specified flags
                newConfiguration |= configurationFlags;
            }
            else
            {
                // reset specified flags
                newConfiguration &= ~configurationFlags;
            }

            await WriteConfigurationAsync(newConfiguration).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Ds2482StatusFlags> WaitOnBusyAsync()
        {
            // TODO: This number is at best a bit random
            for (int i = 100000; i > 0; i--)
            {
                await i2cDevice.WriteReadAsync(readStatusWriteBuffer, readStatusReadBuffer).ConfigureAwait(false);
                if ((readStatusReadBuffer[0] & (byte)Ds2482StatusFlags.OneWireBusy) == 0)
                    break;
            }

            if ((readStatusReadBuffer[0] & (byte)Ds2482StatusFlags.OneWireBusy) != 0)
                throw new Exception("WaitOnBusy timeout.");

            return (Ds2482StatusFlags)readStatusReadBuffer[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public async Task WriteConfigurationAsync(Ds2482ConfigurationFlags configuration)
        {
            byte configurationWithComplement = (byte)((byte)configuration | ((byte)~configuration) << 4);

            await WaitOnBusyAsync();
            await WriteRawAsync(Ds2482CommandCommands.WriteConfiguration, (byte)configurationWithComplement).ConfigureAwait(false);

            // check updated config
            var newConfiguration = await ReadConfigurationAsync().ConfigureAwait(false);
            if (newConfiguration != configuration)
                throw new Exception("WriteConfig failed.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task WriteRawAsync(params byte[] data)
        {
            var res = await i2cDevice.WritePartialAsync(data).ConfigureAwait(false);
            if (res.Status != I2cTransferStatus.FullTransfer)
                throw new Exception("Write failed.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="power"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task<IOneWireBus> OpenOneWireBusAsync(byte? pin = null, bool power = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (pin != null)
                throw new ArgumentOutOfRangeException(nameof(pin), "The Ds2482 does not support pins.");

            return Task.FromResult<IOneWireBus>(new Ds2482OneWireBus(this));
        }
    }
}
