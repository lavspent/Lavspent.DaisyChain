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

using Lavspent.DaisyChain.I2c;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Devices.TexasInstruments.Tca9548A
{
    /// <summary>
    /// 
    /// </summary>
    public class Tca9548A
    {
        private II2cDevice _i2cDevice;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i2cDevice"></param>
        public Tca9548A(II2cDevice i2cDevice)
        {
            _i2cDevice = i2cDevice;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public async Task SelectAsync(byte output, CancellationToken cancellationToken)
        {
            if (output > 7)
                throw new ArgumentException("Must be a number from 0 to 7", nameof(output));

            await _i2cDevice.WriteAsync(new byte[] { (byte)(1 << output) }, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public II2cController GetI2cController(byte output)
        {
            return new Tca9548AI2cController(this, _i2cDevice, output);
        }
    }
}
