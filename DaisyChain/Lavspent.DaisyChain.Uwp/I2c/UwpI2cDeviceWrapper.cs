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


using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.I2c
{
    /// <summary>
    /// Wraps a native I2cDevice with our public II2cDevice interface.
    /// </summary>
    internal class UwpI2cDeviceWrapper : II2cDevice
    {
        private Windows.Devices.I2c.I2cDevice _i2cDevice;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i2cDevice"></param>
        public UwpI2cDeviceWrapper(Windows.Devices.I2c.I2cDevice i2cDevice)
        {
            _i2cDevice = i2cDevice;
        }

        /// <summary>
        /// 
        /// </summary>
        public string DeviceId
        {
            get
            {
                return _i2cDevice.DeviceId;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DaisyChain.I2c.I2cConnectionSettings ConnectionSettings
        {
            get
            {
                // TODO: int to byte conversion, check overflow
                var uwpSettings = _i2cDevice.ConnectionSettings;
                var settings = new DaisyChain.I2c.I2cConnectionSettings((byte)uwpSettings.SlaveAddress)
                {
                    BusSpeed = (DaisyChain.I2c.I2cBusSpeed)uwpSettings.BusSpeed,
                    SharingMode = (DaisyChain.I2c.I2cSharingMode)uwpSettings.SharingMode,
                };
                return settings;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task ReadAsync(byte[] buffer, CancellationToken cancellationToken)
        {
            _i2cDevice.Read(buffer);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task<DaisyChain.I2c.I2cTransferResult> ReadPartialAsync(byte[] buffer, CancellationToken cancellationToken)
        {
            var nativeResult = _i2cDevice.ReadPartial(buffer);
            return Task.FromResult(WrapNativeTransferResult(nativeResult));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task WriteAsync(byte[] buffer, CancellationToken cancellationToken)
        {
            _i2cDevice.Write(buffer);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task<DaisyChain.I2c.I2cTransferResult> WritePartialAsync(byte[] buffer, CancellationToken cancellationToken)
        {
            var nativeResult = _i2cDevice.WritePartial(buffer);
            return Task.FromResult(WrapNativeTransferResult(nativeResult));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writeBuffer"></param>
        /// <param name="readBuffer"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task WriteReadAsync(byte[] writeBuffer, byte[] readBuffer, CancellationToken cancellationToken)
        {
            _i2cDevice.WriteRead(writeBuffer, readBuffer);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writeBuffer"></param>
        /// <param name="readBuffer"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task<DaisyChain.I2c.I2cTransferResult> WriteReadPartialAsync(byte[] writeBuffer, byte[] readBuffer, CancellationToken cancellationToken)
        {
            var nativeResult = _i2cDevice.WriteReadPartial(writeBuffer, readBuffer);
            return Task.FromResult(WrapNativeTransferResult(nativeResult));
        }

        private DaisyChain.I2c.I2cTransferResult WrapNativeTransferResult(Windows.Devices.I2c.I2cTransferResult nativeResult)
        {
            DaisyChain.I2c.I2cTransferResult result = new DaisyChain.I2c.I2cTransferResult();
            result.BytesTransferred = nativeResult.BytesTransferred;
            result.Status = (DaisyChain.I2c.I2cTransferStatus)nativeResult.Status;

            return result;
        }
    }
}
