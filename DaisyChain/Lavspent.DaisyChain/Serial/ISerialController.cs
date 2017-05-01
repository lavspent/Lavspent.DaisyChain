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

using System.ComponentModel;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Serial
{
    /// <summary>
    /// A controller that provides a serial device (UART, software serial etc.)
    /// </summary>
    public interface ISerialController
    {
        /// <summary>
        /// Opens a serial device with given port and parameters.
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baud"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        /// <returns></returns>
        Task<ISerial> OpenSerialAsync(PortName portName, int baud, Parity parity, byte dataBits, StopBits stopBits);

        /// <summary>
        /// Opens a serial device with default (or the only one) port and parameters.
        /// </summary>
        /// <param name="baud"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        /// <returns></returns>
        //Task<ISerial> OpenSerialAsync(int baud, Parity parity, byte dataBits, StopBits stopBits);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ISerialControllerEx
    {
        /// Opens a serial device with PortName.None and provided parameters.
        public static Task<ISerial> OpenSerialAsync(this ISerialController _this, int baud, Parity parity, byte dataBits, StopBits stopBits)
        {
            return _this.OpenSerialAsync(PortName.None, baud, parity, dataBits, stopBits);
        }
    }
}
