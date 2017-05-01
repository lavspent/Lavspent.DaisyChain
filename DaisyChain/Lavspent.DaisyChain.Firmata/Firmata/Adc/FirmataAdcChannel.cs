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

using Lavspent.DaisyChain.Adc;
using Lavspent.DaisyChain.Firmata.Messages;
using Lavspent.DaisyChain.Gpio;
using Lavspent.DaisyChain.Misc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Firmata.Adc
{
    public class FirmataAdcChannel : IAdcChannel
    {
        private FirmataClient _firmataClient;
        private int _pinNumber;
        private GpioDriveMode _gpioDriverMode;
        private IDisposable _analogMessageSubscription;
        private IAdcChannelValue _lastValue;
        private Source<IAdcChannelValue> _source;

        public FirmataAdcChannel(FirmataClient firmataClient, int pinNumber)
        {
            _firmataClient = firmataClient;
            _pinNumber = pinNumber;
            _gpioDriverMode = GpioDriveMode.Output;
            _source = new Source<IAdcChannelValue>();
            _analogMessageSubscription = RegisterAnalogMessageObserver(
                new Observer<AnalogMessage>(onAnalogMessage, null, null));
        }

        private void onAnalogMessage(AnalogMessage analogMessage)
        {
            if (analogMessage.Pin == _pinNumber)
            {
                _lastValue = CreateAnalogPinValue(analogMessage.Value);

                // todo: is there a race here? can _lastValue change before it's been read?
                // do we need a struct? or clone?
                _source.Next(_lastValue);
            }
        }

        public int PinNumber
        {
            get
            {
                return _pinNumber;
            }
        }

        public int MinValue
        {
            get
            {
                // todo: IS this true?
                return 0;
            }
        }

        public int MaxValue
        {
            get
            {
                // todo: this depends on Firmata configuration, right?
                return 1023;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Todo: when this instance was opened analog reporting was requested 
            // for this "pin". As long as this instance lives there should be
            // a mechanism for hindering anyone from stopping analog reporting
            // on that pin. Now, as we're disposing that hinder should cease.

            if (disposing)
            {
                _analogMessageSubscription.Dispose();
            }
        }

        public Task<GpioDriveMode> GetDriveModeAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsDriveModeSupportedAsync(GpioDriveMode driveMode)
        {
            bool result;
            switch (driveMode)
            {
                case GpioDriveMode.Input:
                    result = true;
                    break;
                default:
                    result = false;
                    break;
            }

            return System.Threading.Tasks.Task.FromResult(result);
        }

        public async Task<IAdcChannelValue> ReadAsync(CancellationToken cancellationToken)
        {
            // todo: hack implementation of waiting for this value to be set
            while (_lastValue == null)
                await System.Threading.Tasks.Task.Yield();

            // todo: we could cache this task, and create a new one if the underlying
            // _lastValue changes.
            return _lastValue;
        }

        private IAdcChannelValue CreateAnalogPinValue(int value)
        {
            return new AdcChannelValue()
            {
                Value = value,
                Ratio = ((double)(value - this.MinValue)) / ((double)(this.MaxValue - this.MinValue))
            };
        }

        //public Task SetDriveModeAsync(GpioDriveMode driveMode)
        //{
        //    var pinMode = PinModeFromDriveMode(driveMode);
        //    return _firmataClient.SetDigitalPinModeAsync((byte)_pinNumber, pinMode);
        //}

        private PinMode PinModeFromDriveMode(GpioDriveMode driveMode)
        {
            switch (driveMode)
            {
                case GpioDriveMode.Input:
                    return PinMode.Input;
                case GpioDriveMode.Output:
                    return PinMode.Output;
                default:
                    throw new Exception("Not supported.");
            }
        }

        public IDisposable Subscribe(IObserver<IAdcChannelValue> observer)
        {
            return _source.Subscribe(observer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        /// todo: Move to controller?
        public IDisposable RegisterAnalogMessageObserver(IObserver<AnalogMessage> observer)
        {
            return _firmataClient.RegisterResponseObserver(AnalogMessage.MessageDescriptor, observer);
        }
    }
}
