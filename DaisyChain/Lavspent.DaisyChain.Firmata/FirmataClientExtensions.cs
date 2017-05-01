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
using Lavspent.DaisyChain.Devices;
using Lavspent.DaisyChain.Gpio;
using Lavspent.DaisyChain.I2c;
using Lavspent.DaisyChain.OneWire;
using Lavspent.DaisyChain.Serial;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Firmata
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class FirmataClientExtensions
    {

        /// <summary>
        /// Get the Gpio controller for this Firmata client.        
        /// </summary>
        /// <param name="_this"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<IGpioController> GetGpioControllerAsync(this Task<FirmataClient> _this, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await (await _this.ConfigureAwait(false)).GetGpioControllerAsync(cancellationToken).ConfigureAwait(false);
        }


        /// <summary>
        /// Get the Adc controller for this Firmata client.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static async Task<IAdcController> GetAdcControllerAsync(this Task<FirmataClient> _this, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await (await _this.ConfigureAwait(false)).GetAdcControllerAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the I2c controller for this Firmata client.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static async Task<II2cController> GetI2cControllerAsync(this Task<FirmataClient> _this, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await (await _this.ConfigureAwait(false)).GetI2cControllerAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the serial controller for this Firmata client.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static async Task<ISerialController> GetSerialControllerAsync(this Task<FirmataClient> _this)
        {
            // todo: make cancellable
            return await (await _this.ConfigureAwait(false)).GetSerialControllerAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static async Task<IEncoderController> GetEncoderControllerAsync(this Task<FirmataClient> _this)
        {
            // TODO: Make this cancellable
            return await (await _this.ConfigureAwait(false)).GetEncoderControllerAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static async Task<IOneWireBusController> GetOneWireBusControllerAsync(this Task<FirmataClient> _this)
        {
            // TODO: Make this cancellable
            return await (await _this.ConfigureAwait(false)).GetOneWireBusControllerAsync().ConfigureAwait(false);
        }
    }
}
