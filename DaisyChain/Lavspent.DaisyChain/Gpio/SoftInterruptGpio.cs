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

using Lavspent.DaisyChain.Misc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Gpio
{
    /// <summary>
    /// Simulates interrupts by polling gpio value and reporting changes.
    /// Used for Gpio's that doesn't support hardware interrupts natively.
    /// </summary>
    // TODO: We probably need some magic here. It's too expensive to poll repeatedly,
    // epescially when over usb/firmata/wifi whatever... 
    // Maybe the underlying "setup" can provide a better, proprietary, specially designed
    // and softer softinterrupt?
    internal class SoftInterruptGpio : IGpio
    {
        private IGpio _gpio;
        private bool _disposeGpio;
        private GpioValue _lastGpioValue;
        private Source<GpioEdge> _source;

        private bool _run = true;

        public SoftInterruptGpio(IGpio gpio, bool disposeGpio = true)
        {
            _gpio = gpio;
            _disposeGpio = disposeGpio;
            _source = new Misc.Source<Gpio.GpioEdge>();

            // todo: not right. grab value from pin at startup
            _lastGpioValue = GpioValue.Low;

            IssuePoll();
        }

        private void IssuePoll()
        {
            _gpio.ReadAsync().ContinueWith(
                t =>
                {
                    var gpioValue = t.Result;
                    if (_lastGpioValue != gpioValue)
                    {
                        // get change
                        _lastGpioValue = gpioValue;

                        GpioEdge gpioEdge;

                        if (gpioValue == GpioValue.High)
                        {
                            gpioEdge = GpioEdge.RisingEdge;
                        }
                        else
                        {
                            gpioEdge = GpioEdge.FallingEdge;
                        }

                        // notify observers
                        _source.Next(gpioEdge);
                    }

                    if (_run)
                    {
                        // issue new poll
                        IssuePoll();
                    }
                },
                TaskContinuationOptions.OnlyOnRanToCompletion
                );
        }

        // TODO: Implement proper disposing
        public void Dispose()
        {
            // stop polling
            // todo: not good an racy this one
            _run = false;

            if (_disposeGpio)
            {
                _gpio.Dispose();
            }
        }

        public Task<GpioDriveMode> GetDriveModeAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _gpio.GetDriveModeAsync(cancellationToken);
        }

        public Task<bool> IsDriveModeSupportedAsync(GpioDriveMode driveMode, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _gpio.IsDriveModeSupportedAsync(driveMode, cancellationToken);
        }

        public Task<GpioValue> ReadAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _gpio.ReadAsync(cancellationToken);
        }

        public Task SetDriveModeAsync(GpioDriveMode value, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _gpio.SetDriveModeAsync(value, cancellationToken);
        }

        public IDisposable Subscribe(IObserver<GpioEdge> observer)
        {
            return _source.Subscribe(observer);
        }

        public Task WriteAsync(GpioValue value, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _gpio.WriteAsync(value, cancellationToken);
        }
    }
}
