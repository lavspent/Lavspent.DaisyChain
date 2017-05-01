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

using Lavspent.DaisyChain.Firmata.Messages;
using Lavspent.DaisyChain.Gpio;
using Lavspent.DaisyChain.Misc;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Firmata.Gpio
{
    internal class FirmataGpio : IGpio
    {
        private FirmataGpioController _gpioController;
        private int _pinNumber;
        private GpioDriveMode _gpioDriverMode;
        private IDisposable _digitalMessageSubscription;
        private Source<GpioEdge> _source;
        private GpioValue _lastValue;


        public FirmataGpio(FirmataGpioController gpioController, int pinNumber)
        {
            _gpioController = gpioController;
            _pinNumber = pinNumber;
            _gpioDriverMode = GpioDriveMode.Output;
            _source = new Source<GpioEdge>();

            _digitalMessageSubscription = _gpioController.RegisterDigitalMessageObserver(
                new Observer<DigitalMessage>(OnDigitalMessage));

            _lastValue = GpioValue.Low;
        }

        private void OnDigitalMessage(DigitalMessage digitalMessage)
        {
            Debug.WriteLine($"{digitalMessage.Pin} {digitalMessage.Value}");

            byte bits = (byte)((digitalMessage.Data[0] & 0x7f) | (digitalMessage.Data[1] << 6));

            GpioValue value = (((bits >> _pinNumber) & 0x01) == 0x01) ? GpioValue.High : GpioValue.Low;
            if (_lastValue != value)
            {
                _lastValue = value;
                _source.Next(value == GpioValue.High ? GpioEdge.RisingEdge : GpioEdge.FallingEdge);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _digitalMessageSubscription.Dispose();
            }
        }

        public Task<GpioDriveMode> GetDriveModeAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsDriveModeSupportedAsync(GpioDriveMode driveMode, CancellationToken cancellationToken)
        {
            bool result;
            switch (driveMode)
            {
                case GpioDriveMode.Input:
                case GpioDriveMode.Output:
                    result = true;
                    break;
                default:
                    result = false;
                    break;
            }

            return System.Threading.Tasks.Task.FromResult(result);
        }

        public IDisposable Lock()
        {
            throw new NotImplementedException();
        }

        public async Task<GpioValue> ReadAsync(CancellationToken cancellationToken)
        {
            var value = await _gpioController.ReadDigitalPinValueAsync(_pinNumber, cancellationToken).ConfigureAwait(false);
            return value;
        }

        public Task SetDriveModeAsync(GpioDriveMode driveMode, CancellationToken cancellationToken)
        {
            var pinMode = PinModeFromDriveMode(driveMode);
            return _gpioController.SetDigitalPinModeAsync((byte)_pinNumber, pinMode, cancellationToken);
        }

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

        public Task WriteAsync(GpioValue value, CancellationToken cancellationToken)
        {
            return _gpioController.SetDigitalPinValueAsync((byte)_pinNumber, value, cancellationToken);
        }

        public IDisposable Subscribe(IObserver<GpioEdge> observer)
        {
            //_gpioController.ReportDigitalPinAsync((byte)_pinNumber, true).Wait();
            // Turn on analog reporting
            //await _firmataClient.ReportAnalogPinAsync(0, true);

            return _source.Subscribe(observer);
        }
    }
}
