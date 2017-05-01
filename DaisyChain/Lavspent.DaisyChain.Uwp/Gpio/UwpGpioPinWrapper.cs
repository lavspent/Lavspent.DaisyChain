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
    /// A wrapper for the native GpioPin allowing us to treat is as our public IGpioPin.
    /// </summary>
    internal class UwpGpioPinWrapper : IGpio
    {
        private Windows.Devices.Gpio.GpioPin _gpioPin;
        private Source<GpioEdge> _source;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gpioPin"></param>
        public UwpGpioPinWrapper(Windows.Devices.Gpio.GpioPin gpioPin)
        {
            _gpioPin = gpioPin;
            _source = new Source<GpioEdge>();
            _gpioPin.ValueChanged += _gpioPin_ValueChanged;
        }

        private void _gpioPin_ValueChanged(object sender, Windows.Devices.Gpio.GpioPinValueChangedEventArgs args)
        {
            _source.Next((GpioEdge)args.Edge);
        }


        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
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
                _gpioPin.Dispose();
                _gpioPin = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task<DaisyChain.Gpio.GpioDriveMode> GetDriveModeAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((DaisyChain.Gpio.GpioDriveMode)_gpioPin.GetDriveMode());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driveMode"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task<bool> IsDriveModeSupportedAsync(DaisyChain.Gpio.GpioDriveMode driveMode, CancellationToken cancellationToken)
        {
            return Task.FromResult(_gpioPin.IsDriveModeSupported((Windows.Devices.Gpio.GpioPinDriveMode)driveMode));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task<DaisyChain.Gpio.GpioValue> ReadAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((DaisyChain.Gpio.GpioValue)_gpioPin.Read());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task SetDriveModeAsync(DaisyChain.Gpio.GpioDriveMode value, CancellationToken cancellationToken)
        {
            _gpioPin.SetDriveMode((Windows.Devices.Gpio.GpioPinDriveMode)value);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task WriteAsync(DaisyChain.Gpio.GpioValue value, CancellationToken cancellationToken)
        {
            _gpioPin.Write((Windows.Devices.Gpio.GpioPinValue)value);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<GpioEdge> observer)
        {
            return _source.Subscribe(observer);
        }
    }
}
