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

using Lavspent.AsyncInline;
using Windows.Devices.I2c.Provider;

namespace Lavspent.DaisyChain.I2c
{
    internal class UwpII2cDeviceProvider : II2cDeviceProvider
    {
        private II2cDevice _i2cDevice;

        public UwpII2cDeviceProvider(II2cDevice i2cDevice)
        {
            _i2cDevice = i2cDevice;
        }

        public string DeviceId
        {
            get
            {
                return "DaisyChain"; // _i2cDevice.DeviceId;
            }
        }

        public void Dispose()
        {
            _i2cDevice = null;
        }

        public void Read(byte[] buffer)
        {
            _i2cDevice.ReadAsync(buffer).WaitInline();
        }

        public ProviderI2cTransferResult ReadPartial(byte[] buffer)
        {
            var i2cTransferResult = _i2cDevice.ReadPartialAsync(buffer).WaitInline();
            return I2cUtils.ProviderI2cTransferResultFromI2cTransferResult(i2cTransferResult);
        }

        public void Write(byte[] buffer)
        {
            _i2cDevice.WriteAsync(buffer).WaitInline();
        }

        public ProviderI2cTransferResult WritePartial(byte[] buffer)
        {
            var i2cTransferResult = _i2cDevice.WritePartialAsync(buffer).WaitInline();
            return I2cUtils.ProviderI2cTransferResultFromI2cTransferResult(i2cTransferResult);
        }

        public void WriteRead(byte[] writeBuffer, byte[] readBuffer)
        {
            _i2cDevice.WriteReadAsync(writeBuffer, readBuffer).Wait();
        }

        public ProviderI2cTransferResult WriteReadPartial(byte[] writeBuffer, byte[] readBuffer)
        {
            var i2cTransferResult = _i2cDevice.WriteReadPartialAsync(writeBuffer, readBuffer).WaitInline();
            return I2cUtils.ProviderI2cTransferResultFromI2cTransferResult(i2cTransferResult);
        }
    }


    //internal static class TaskEx
    //{
    //    public static T WaitForResult<T>(this Task<T> _this)
    //    {
    //        return AsyncInline.AsyncInline.Run(() =>
    //        {
    //            return _this;
    //        });
    //    }
    //}
}
