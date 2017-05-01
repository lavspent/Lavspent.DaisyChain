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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Devices.Microchip.Mcp23017
{
    /// <summary>
    /// 
    /// </summary>
    public class Mcp23017GpioController : IGpioController
    {
        internal Mcp23017Port _port;
        private Dictionary<int, IGpio> _pins;

        public Mcp23017GpioController(Mcp23017Port port)
        {
            _port = port;
            _pins = new Dictionary<int, IGpio>();
        }

        public int GpioCount { get; } = 16;

        /// <summary>
        /// Open pin in specific sharing mode.
        /// </summary>
        /// <param name="gpioNumber">Pin to open.</param>
        /// <param name="sharingMode">Sharing mode.</param>
        /// <returns>IGpio</returns>
        public Task<IGpio> OpenGpioAsync(int gpioNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            IGpio gpio = null;

            if (gpioNumber >= GpioCount)
            {
                throw new InvalidOperationException("Pin unavailable.");
            }

            if (!_pins.ContainsKey(gpioNumber))
            {
                gpio = _pins[gpioNumber] = new Mcp23017Gpio(this, gpioNumber);
            }
            else
            {
                throw new InvalidOperationException("Sharing violation occurred.");
            }

            return Task.FromResult(gpio);
        }

        internal void ClosePin(Mcp23017Gpio gpio)
        {
            // todo: dirty coupling?
            _pins[gpio._pinNumber] = null;
        }
    }
}
