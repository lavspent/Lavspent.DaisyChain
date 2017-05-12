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
    internal class FirmataI2cController : II2cController, IDisposable
    {
        private FirmataClient _firmataClient;
        public string DeviceId { get; private set; }

        public FirmataI2cController(FirmataClient firmataClient)
        {
            _firmataClient = firmataClient;

            DeviceId = I2cDeviceIdTool.NewDeviceId();
            I2cControllerManager.Instance.Register(DeviceId, this);
        }

        public Task<II2cDevice> OpenDeviceAsync(I2cConnectionSettings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult<II2cDevice>(new FirmataI2cDevice(_firmataClient, this, settings));
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                I2cControllerManager.Instance.Unregister(DeviceId);

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
