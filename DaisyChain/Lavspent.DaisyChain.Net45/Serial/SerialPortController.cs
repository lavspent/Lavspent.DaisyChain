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

using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Serial
{
    /// <summary>
    /// A "host" controller for .Net 4.5 SerialPorts.
    /// </summary>
    public class SerialPortController : ISerialController
    {
        private static object _lock = new object();
        private static SerialPortController _serialPortController;

        /// <summary>
        /// Opens a new SerialPort from the default SerialPortController.
        /// </summary>
        /// <returns></returns>
        public static Task<ISerial> OpenSerialAsync(string portName, int baudRate, Parity parity, byte dataBits, StopBits stopBits, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetDefault().OpenSerialAsync(portName, baudRate, parity, dataBits, stopBits);
        }

        /// <summary>
        ///  Get the default SerialPortController
        /// </summary>
        /// <returns></returns>
        public static ISerialController GetDefault()
        {
            lock (_lock)
            {
                if (_serialPortController == null)
                    _serialPortController = new SerialPortController();
            }

            return _serialPortController;
        }

        /// <summary>
        /// Opens a SerialPort on this SerialPortController.
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baud"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        /// <returns></returns>
        async Task<ISerial> ISerialController.OpenSerialAsync(PortName portName, int baud, Parity parity, byte dataBits, StopBits stopBits)
        {
            var serialPort = new SerialPort(portName, baud, parity.AsNativeParity(), dataBits, stopBits.AsNativeStopBits());
            var serial = serialPort.AsDaisyChainSerial();
            await serial.OpenAsync();
            return serial;
        }
    }
}
