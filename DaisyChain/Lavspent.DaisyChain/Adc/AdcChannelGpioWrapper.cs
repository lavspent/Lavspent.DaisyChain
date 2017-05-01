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
using Lavspent.DaisyChain.Misc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Adc
{
    /// <summary>
    /// Wraps a adc channel and provides a IGpio interface.
    /// </summary>
    internal class AdcChannelGpioWrapper : IGpio, IDisposable
    {
        /// <summary>
        /// The AdcChannel we're wrapping.
        /// </summary>
        private IAdcChannel _adcChannel;

        /// <summary>
        /// 
        /// </summary>
        private double _lowerThreshold;

        /// <summary>
        /// 
        /// </summary>
        private double _upperThreshold;

        /// <summary>
        /// 
        /// </summary>
        private bool _disposeAdcChannel;

        /// <summary>
        /// 
        /// </summary>
        private IDisposable _adcChannelSubscription;

        /// <summary>
        /// 
        /// </summary>
        private Source<GpioEdge> _source;

        /// <summary>
        /// 
        /// </summary>
        private GpioValue _lastGpioValue;


        public AdcChannelGpioWrapper(IAdcChannel adcChannel, bool disposeAdcChannel = true)
            : this(adcChannel, 0.45d, 0.55d, disposeAdcChannel)
        {
        }

        public AdcChannelGpioWrapper(IAdcChannel adcChannel, double lowerThreshold, double upperThreshold, bool disposeAdcChannel = true)
        {
            _adcChannel = adcChannel ?? throw new ArgumentNullException(nameof(adcChannel));
            if (lowerThreshold >= upperThreshold)
                throw new ArgumentOutOfRangeException($"{nameof(lowerThreshold)} must be smaller than {nameof(upperThreshold)}.");

            _lowerThreshold = lowerThreshold;
            _upperThreshold = upperThreshold;
            _disposeAdcChannel = disposeAdcChannel;

            _source = new Source<GpioEdge>();

            _adcChannelSubscription = _adcChannel.Subscribe(
                new Observer<IAdcChannelValue>(OnObservation));
        }


        private void OnObservation(IAdcChannelValue adcChannelValue)
        {
            DisposedUtil.Check(this, disposed);

            GpioValue gpioValue = GetGpioValue(adcChannelValue);
            if (_lastGpioValue != gpioValue)
            {
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

                _source.Next(gpioEdge);
            }
        }


        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task<GpioDriveMode> GetDriveModeAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            DisposedUtil.Check(this, disposed);
            throw new NotSupportedException("AdcChannels does not support drive mode.");
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="driveMode"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task<bool> IsDriveModeSupportedAsync(GpioDriveMode driveMode, CancellationToken cancellationToken = default(CancellationToken))
        {
            DisposedUtil.Check(this, disposed);
            throw new NotSupportedException("AdcChannels does not support drive mode.");
        }

        public async Task<GpioValue> ReadAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            DisposedUtil.Check(this, disposed);
            var analogValue = await _adcChannel.ReadAsync(cancellationToken);
            return GetGpioValue(analogValue);
        }

        private GpioValue GetGpioValue(IAdcChannelValue analogGpioValue)
        {
            DisposedUtil.Check(this, disposed);
            if (analogGpioValue == null)
                throw new ArgumentNullException(nameof(analogGpioValue));

            var gpioValue = analogGpioValue.Ratio <= _lowerThreshold ? GpioValue.Low :
                            analogGpioValue.Ratio >= _upperThreshold ? GpioValue.High :
                            _lastGpioValue;

            return gpioValue;

        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task SetDriveModeAsync(GpioDriveMode value, CancellationToken cancellationToken = default(CancellationToken))
        {
            DisposedUtil.Check(this, disposed);
            throw new NotSupportedException("AdcChannels does not support drive mode.");
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task WriteAsync(GpioValue value, CancellationToken cancellationToken = default(CancellationToken))
        {
            DisposedUtil.Check(this, disposed);
            throw new NotSupportedException("AdcChannels does not support writing.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<GpioEdge> observer)
        {
            DisposedUtil.Check(this, disposed);
            return _source.Subscribe(observer);
        }

        #region IDisposable
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                _adcChannelSubscription.Dispose();

                if (_disposeAdcChannel)
                {
                    _adcChannel.Dispose();
                }
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }
}
