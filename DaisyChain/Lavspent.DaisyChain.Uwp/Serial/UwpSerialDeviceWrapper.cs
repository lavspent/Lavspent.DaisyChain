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

using Lavspent.DaisyChain.Stream;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.SerialCommunication;

namespace Lavspent.DaisyChain.Serial
{
    /// <summary>
    /// Wraps a UWP ServialDevice as a DaisyChain ISerial
    /// </summary>
    internal class UwpSerialDeviceWrapper : ISerial, IDisposable
    {
        private SerialDevice _serialDevice;
        private IStream _stream;
        private bool _disposeSerialDevice;

        public UwpSerialDeviceWrapper(SerialDevice serialDevice, bool disposeSerialDevice = true)
        {
            _serialDevice = serialDevice;
            _disposeSerialDevice = disposeSerialDevice;
        }

        public ushort DataBits
        {
            get
            {
                return _serialDevice.DataBits;
            }

            set
            {
                _serialDevice.DataBits = value;
            }
        }

        public Parity Parity
        {
            get
            {
                return _serialDevice.Parity.AsParity();
            }

            set
            {
                _serialDevice.Parity = value.AsNativeSerialParity();
            }
        }

        public StopBits StopBits
        {
            get
            {
                return _serialDevice.StopBits.AsStopBits();
            }

            set
            {
                _serialDevice.StopBits = value.AsNativeSerialStopBitCount();
            }
        }

        public Handshake Handshake
        {
            get
            {
                return _serialDevice.Handshake.AsHandshake();
            }

            set
            {
                _serialDevice.Handshake = value.AsNativeSerialHandshake();
            }
        }

        public Task CloseAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // to close the device we need to dispose the SerialDevice
            this.Dispose();
            return Task.CompletedTask;
        }

        public Task OpenAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // A SerialDevice comes open out of the controller.
            return Task.CompletedTask;
        }

        public IStream GetStream()
        {
            if (_stream == null)
            {
                _stream = new UwpSerialDeviceStreamWrapper(_serialDevice);
            }

            return _stream;
        }

        public void Dispose()
        {
            if (_disposeSerialDevice)
            {
                _serialDevice?.Dispose();
                _serialDevice = null;
            }
        }
    }
}