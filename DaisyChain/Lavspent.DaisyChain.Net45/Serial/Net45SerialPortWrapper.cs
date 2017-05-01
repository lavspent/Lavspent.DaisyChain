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
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Serial
{
    /// <summary>
    /// A wrapper class that wraps .Net 4.5 SerialPort and gives them an DaisyChain ISerial interface.
    /// </summary>
    internal class Net45SerialPortWrapper : ISerial
    {
        private SerialPort _serialPort;
        private IStream _stream;
        private bool _disposeSerialPort;

        public Net45SerialPortWrapper(SerialPort serialPort, bool disposeSerialPort = true)
        {
            _serialPort = serialPort;
            _disposeSerialPort = disposeSerialPort;
        }

        public ushort DataBits
        {
            get
            {
                if (_serialPort.DataBits > ushort.MaxValue)
                    throw new Exception("Overflow");

                return (ushort)_serialPort.DataBits;
            }

            set
            {
                _serialPort.DataBits = value;
            }
        }

        public Serial.Parity Parity
        {
            get
            {
                // todo: fix conversion
                return (Serial.Parity)_serialPort.Parity;
            }

            set
            {
                // todo: fix conversion
                _serialPort.Parity = (System.IO.Ports.Parity)value;
            }
        }

        public Serial.StopBits StopBits
        {
            get
            {
                // todo: fix conversion
                return (Serial.StopBits)_serialPort.StopBits;
            }

            set
            {
                // todo: fix conversion
                _serialPort.StopBits = (System.IO.Ports.StopBits)value;
            }

        }


        public Handshake Handshake
        {
            get
            {
                // todo: fix conversion
                return (Handshake)_serialPort.Handshake;
            }

            set
            {
                // todo: fix conversion
                _serialPort.Handshake = (System.IO.Ports.Handshake)value;
            }
        }

        public IStream GetStream()
        {
            if (_stream == null)
            {
                _stream = _serialPort.BaseStream.AsDaisyChainStream();
            }

            return _stream;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task OpenAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => _serialPort.Open(), cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task CloseAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => _serialPort.Close(), cancellationToken);
        }

        public void Dispose()
        {
            if (_disposeSerialPort)
            {
                _serialPort?.Dispose();
                _serialPort = null;
            }
        }
    }
}
