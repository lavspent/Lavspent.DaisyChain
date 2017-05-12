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
    /// <summary>
    /// Wraps a DaisyChain II2cController an a UWP II2cControllerProvider
    /// </summary>
    internal class UwpII2cControllerProvider : II2cControllerProvider
    {
        private II2cController _i2cController;

        public UwpII2cControllerProvider(II2cController i2cController)
        {
            _i2cController = i2cController;
        }

        public II2cDeviceProvider GetDeviceProvider(ProviderI2cConnectionSettings settings)
        {
            // open device
            // TODO: int to byte conversion, check overflow
            var device = _i2cController.OpenDeviceAsync(
                new I2cConnectionSettings((byte)settings.SlaveAddress)
                {
                    BusSpeed = (I2cBusSpeed)settings.BusSpeed,
                    SharingMode = (I2cSharingMode)settings.SharingMode
                }).WaitInline();

            return new UwpII2cDeviceProvider(device);
        }
    }
}
