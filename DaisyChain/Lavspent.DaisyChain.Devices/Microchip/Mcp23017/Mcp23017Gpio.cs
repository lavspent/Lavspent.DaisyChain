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

using Lavspent.Backport;
using Lavspent.DaisyChain.Gpio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Devices.Microchip.Mcp23017
{
    public class Mcp23017Gpio : IGpio
    {
        private Mcp23017GpioController _controller;
        //        private Mcp23017 _device;

        private Mcp23017Port _port;
        private byte _pinMask;
        private Mcp23017ControlRegisters _registers;

        // this: fix this internal
        internal int _pinNumber;

        public Mcp23017Gpio(Mcp23017GpioController controller, int pinNumber)
        {
            _controller = controller;
            _port = _controller._port;
            //_device = controller._device;
            _pinNumber = pinNumber;
            _pinMask = (byte)(0x01 << _pinNumber);

            //if (PinNumber < 8)
            //{
            //    _port = Mcp23017PortEnum.A;
            //    _pinMask = (byte)(0x01 << PinNumber);
            //}
            //else
            //{
            //    _port = Mcp23017PortEnum.B;
            //    _pinMask = (byte)(0x01 << (PinNumber - 8));
            //}

            //_registers = new Mcp23017ControlRegisters(_port, controller._device);

            //_lock = new DisposableLock()
            //{
            //    PostLock = PostLock,
            //    PreUnlock = PreUnlock
            //};
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
                case GpioDriveMode.InputPullUp:
                case GpioDriveMode.InputPullDown:
                case GpioDriveMode.OutputOpenDrain:
                case GpioDriveMode.OutputOpenDrainPullUp:
                case GpioDriveMode.OutputOpenSource:
                case GpioDriveMode.OutputOpenSourcePullDown:
                default:
                    result = false;
                    break;
            }

            return Task.FromResult(result);
        }

        public Task<GpioDriveMode> GetDriveModeAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task SetDriveModeAsync(GpioDriveMode value, CancellationToken cancellationToken)
        {
            if (!await IsDriveModeSupportedAsync(value, cancellationToken))
                throw new ArgumentException("Drive mode not supported.");

            /*            switch (value)
                        {
                            case GpioPinDriveMode.Output:
                                byte ioDirection
                                _device.IoDirectionARegister =  _device.IoDirectionARegister
                        }
            */
            throw new NotImplementedException();
        }

        public Task<GpioValue> ReadAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((_port.Gpio & _pinMask) != 0 ? GpioValue.High : GpioValue.Low);
        }

        public Task WriteAsync(GpioValue value, CancellationToken cancellationToken)
        {
            byte currentValue, newValue;

            // get value of other pins so we don't mess them up
            currentValue = _port.OLat;

            // flip bits
            if (value == GpioValue.High)
                newValue = (byte)(currentValue | _pinMask);
            else
                newValue = (byte)(currentValue & ~_pinMask);

            // write if necessary
            if (currentValue != newValue)
                _port.OLat = newValue;

            return TaskEx.CompletedTask;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _controller.ClosePin(this);
            }
        }

        public IDisposable Subscribe(IObserver<GpioEdge> observer)
        {
            throw new NotImplementedException();
        }
    }
}
