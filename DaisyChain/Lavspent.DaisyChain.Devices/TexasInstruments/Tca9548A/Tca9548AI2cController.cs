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
    internal class Tca9548AI2cController : II2cController, IDisposable
    {
        private Tca9548A _tca9548A;
        private I2c.II2cDevice _i2cDevice;
        private byte _output;
        private string _deviceId;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tca9548A"></param>
        /// <param name="i2cDevice"></param>
        /// <param name="output"></param>
        public Tca9548AI2cController(Tca9548A tca9548A, II2cDevice i2cDevice, byte output)
        {
            _tca9548A = tca9548A;
            _i2cDevice = i2cDevice;
            _output = output;
            _deviceId = i2cDevice.DeviceId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public async Task<II2cDevice> OpenDeviceAsync(I2cConnectionSettings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (settings.SlaveAddress == _i2cDevice.ConnectionSettings.SlaveAddress)
                throw new Exception("Tcs9548A has same address.");

            // The Tca9548A multiplexes the output when told to, other than that it's quite transparent.
            // To talk to one of it's "children" we actually have to grab them through the parent controller.

            // grab parent controller, and get a device from there
            II2cController parentI2cController = I2cControllerManager.Instance.GetI2cController(_deviceId);
            II2cDevice i2cDevice = await parentI2cController.OpenDeviceAsync(settings, cancellationToken).ConfigureAwait(false);

            return new Tca9548AI2cDevice(_tca9548A, i2cDevice, _output, settings, _deviceId);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {

            }

            I2cControllerManager.Instance.Unregister(_deviceId);
        }
    }
}
