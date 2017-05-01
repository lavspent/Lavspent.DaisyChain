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

using Lavspent.DaisyChain.Gpio;
using System;
using System.Threading.Tasks;
using System.Threading;
using Lavspent.DaisyChain.I2c;

namespace Lavspent.DaisyChain.Devices.TexasInstruments.Pcf8574
{
    /// <summary>
    /// Pcf8574 8-bit I/O expander
    /// </summary>
    class Pcf8574 : IGpioController
    {
        internal II2cDevice _i2cDevice;

        public int GpioCount
        {
            get
            {
                return 8;
            }
        }

        public Pcf8574(II2cDevice i2cDevice)
        {
            _i2cDevice = i2cDevice;
        }

        public Task<IGpio> OpenGpioAsync(int gpioNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            // TODO: Fix sharing?
            return Task.FromResult<IGpio>(new Pcf8574Gpio(this, gpioNumber));

        }
    }
}