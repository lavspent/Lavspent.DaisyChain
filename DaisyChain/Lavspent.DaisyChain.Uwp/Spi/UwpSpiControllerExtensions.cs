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

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Devices.Spi;
using Windows.Foundation;

namespace Lavspent.DaisyChain.Spi
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class UwpSpiControllerExtensions
    {
        /// <summary>
        /// Convert this SpiController to a DaisyChain compatible ISpiController.
        /// </summary>
        /// <param name="_this"></param>
        /// <param name="chipSelectLine"></param>
        /// <returns></returns>
        public static ISpiController AsDaisyChainSpiController(this SpiController _this, int chipSelectLine)
        {
            return new UwpSpiControllerWrapper(_this, chipSelectLine);
        }

        /// <summary>
        /// Convert this SpiController to a DaisyChain compatible ISpiController.
        /// </summary>
        /// <param name="_this"></param>
        /// <param name="chipSelectLine"></param>
        /// <returns></returns>        
        public static Task<ISpiController> AsDaisyChainSpiControllerAsync(this IAsyncOperation<SpiController> _this, int chipSelectLine)
        {
            return _this.AsTask().AsDaisyChainSpiControllerAsync(chipSelectLine);
        }

        /// <summary>
        /// Convert this SpiController to a DaisyChain compatible ISpiController.
        /// </summary>
        /// <param name="_this"></param>
        /// <param name="chipSelectLine"></param>
        /// <returns></returns>
        public static async Task<ISpiController> AsDaisyChainSpiControllerAsync(this Task<SpiController> _this, int chipSelectLine)
        {
            return (await _this.ConfigureAwait(false)).AsDaisyChainSpiController(chipSelectLine);
        }
    }
}
