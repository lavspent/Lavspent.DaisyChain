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
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.I2c
{
    /// <summary>
    /// Extends II2cDevices with some commonly used methods.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class II2cDeviceExtensions
    {
        /// <summary>
        /// Write value to a register. Short for writing two bytes, first the register
        /// then the value.
        /// </summary>
        /// <param name="_this"></param>
        /// <param name="register"></param>
        /// <param name="value"></param>
        public static Task WriteRegisterAsync(this II2cDevice _this, byte register, byte value)
        {
            return _this.WriteAsync(new byte[] { register, value });
        }

        /// <summary>
        /// Read value from a register. Short for first writing the register, the sending a 1-byte
        /// read request.
        /// </summary>
        /// <param name="_this"></param>
        /// <param name="register"></param>
        /// <returns></returns>
        public async static Task<byte> ReadRegisterAsync(this II2cDevice _this, byte register, CancellationToken cancellationToken = default(CancellationToken))
        {
            byte[] readBuffer = new byte[1];
            await _this.WriteReadAsync(new byte[] { register }, readBuffer, cancellationToken).ConfigureAwait(false);
            return readBuffer[0];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="_this"></param>
        /// <param name="register"></param>
        /// <param name="bit"></param>
        /// <param name="value"></param>
        public async static Task WriteRegisterBitAsync(this II2cDevice _this, byte register, byte bit, bool value)
        {
            byte currentValue = await _this.ReadRegisterAsync(register).ConfigureAwait(false);
            byte newValue;
            if (value)
            {
                newValue = (byte)(currentValue | 1 << bit);
            }
            else
            {
                newValue = (byte)(currentValue & ~(1 << bit));
            }
            await _this.WriteRegisterAsync(register, newValue).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_this"></param>
        /// <param name="register"></param>
        /// <param name="bit"></param>
        /// <returns></returns>
        public async static Task<bool> ReadRegisterBitAsync(this II2cDevice _this, byte register, byte bit)
        {
            byte value = await _this.ReadRegisterAsync(register).ConfigureAwait(false);
            return ((value >> bit) & 1) == 1;
        }
    }
}
