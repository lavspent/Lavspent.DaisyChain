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
    /// Wraps a UWP I2cController as a DaisyChain II2cController.
    /// </summary>
    internal class UwpI2cControllerWrapper : II2cController
    {
        private Windows.Devices.I2c.I2cController _i2cController;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i2cController"></param>
        public UwpI2cControllerWrapper(Windows.Devices.I2c.I2cController i2cController)
        {
            _i2cController = i2cController;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public Task<II2cDevice> OpenDeviceAsync(I2cConnectionSettings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            var device = _i2cController.GetDevice(new Windows.Devices.I2c.I2cConnectionSettings(settings.SlaveAddress)
            {
                BusSpeed = (Windows.Devices.I2c.I2cBusSpeed)settings.BusSpeed,
                SharingMode = (Windows.Devices.I2c.I2cSharingMode)settings.SharingMode
            }).AsDaisyChainI2cDevice();

            return Task.FromResult<II2cDevice>(device);
        }
    }
}
