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
using Windows.Devices.Gpio.Provider;
using Windows.Foundation;

namespace Lavspent.DaisyChain.Gpio
{
    /// <summary>
    /// Wraps a DaisyChain IGpio as an UWP IGpioPinProvider.
    /// </summary>
    internal class UwpGpioPinProvider : IGpioPinProvider, IDisposable
    {
        private IGpio _gpio;

        public event TypedEventHandler<IGpioPinProvider, GpioPinProviderValueChangedEventArgs> ValueChanged;

        public IDisposable _edgeSubscription;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gpio"></param>
        public UwpGpioPinProvider(IGpio gpio)
        {
            _gpio = gpio;
            _edgeSubscription = gpio.Subscribe(
                new Observer<GpioEdge>(OnGpioEdgeChange, null, null));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gpioPinEdge"></param>
        private void OnGpioEdgeChange(GpioEdge gpioPinEdge)
        {
            // forward the event to anyone listening on the Uwp GpioPin
            ValueChanged?.Invoke(this, new GpioPinProviderValueChangedEventArgs((ProviderGpioPinEdge)gpioPinEdge));
        }

        public int PinNumber
        {
            get
            {
                return 0;   // TODO: Check what happens here. 
                            // DaisyChain doesn't use pins, nor have any knowledge of pin numbers
            }
        }

        public ProviderGpioSharingMode SharingMode
        {
            get
            {
                // All DaisyChain IGpios should be exclusive. 
                return ProviderGpioSharingMode.Exclusive;
            }
        }

        // The Code Analyser complained for some reason about this DebouceTimout
        // Maybe a bug, i don't know
        public TimeSpan DebounceTimeout
        {
            get;
            set;
        }

        public ProviderGpioPinDriveMode GetDriveMode()
        {
            // TODO: needs testing in all kinds of environments
            return (ProviderGpioPinDriveMode)AsyncInline.AsyncInline.Run(
                () => _gpio.GetDriveModeAsync());
        }

        public bool IsDriveModeSupported(ProviderGpioPinDriveMode driveMode)
        {
            // TODO: needs testing in all kinds of environments
            return AsyncInline.AsyncInline.Run(
                () => _gpio.IsDriveModeSupportedAsync((GpioDriveMode)driveMode));
        }

        public void SetDriveMode(ProviderGpioPinDriveMode value)
        {
            // todo: needs testing in all kinds of environments
            AsyncInline.AsyncInline.Run(
                () => _gpio.SetDriveModeAsync((GpioDriveMode)value));
        }

        public ProviderGpioPinValue Read()
        {
            // todo: needs testing in all kinds of environments
            return (ProviderGpioPinValue)AsyncInline.AsyncInline.Run(
                () => _gpio.ReadAsync());
        }
        public void Write(ProviderGpioPinValue value)
        {
            // todo: needs testing in all kinds of environments
            AsyncInline.AsyncInline.Run(
                () => _gpio.WriteAsync((GpioValue)value));
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _edgeSubscription.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
