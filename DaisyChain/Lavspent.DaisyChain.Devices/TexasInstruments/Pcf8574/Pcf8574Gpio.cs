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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lavspent.DaisyChain.Gpio;


namespace Lavspent.DaisyChain.Devices.TexasInstruments.Pcf8574
{
    internal class Pcf8574Gpio : IGpio
    {
        private Pcf8574 _pcf8574;
        private int _gpioNumber;

        public Pcf8574Gpio(Pcf8574 pcf8574, int gpioNumber)
        {
            _pcf8574 = pcf8574;
            _gpioNumber = gpioNumber;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<GpioDriveMode> GetDriveModeAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsDriveModeSupportedAsync(GpioDriveMode driveMode, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task<GpioValue> ReadAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Todo: FIX!
            byte[] buffer = new byte[1];
            await  _pcf8574._i2cDevice.ReadAsync(buffer, cancellationToken);
            return (GpioValue)buffer[0];
        }

        public Task SetDriveModeAsync(GpioDriveMode value, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe(IObserver<GpioEdge> observer)
        {
            throw new NotImplementedException();
        }

        public async Task WriteAsync(GpioValue value, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Todo: FIX!
            byte[] buffer = new byte[1];
            buffer[0] = (byte)value;
            await _pcf8574._i2cDevice.WriteAsync(buffer, cancellationToken);
        }
    }
}
