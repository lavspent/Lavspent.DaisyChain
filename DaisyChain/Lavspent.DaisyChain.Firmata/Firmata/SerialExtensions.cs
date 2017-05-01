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

using Lavspent.DaisyChain.Serial;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Firmata
{
    /// <summary>
    /// Extends II2cDevices with some commonly used methods.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class SerialExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_this"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public static Task<FirmataClient> OpenFirmataClientAsync(this ISerial _this, CancellationToken cancellationToken = default(CancellationToken))
        {
            return FirmataClient.OpenAsync(_this, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_this"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public async static Task<FirmataClient> OpenFirmataClientAsync(this Task<ISerial> _this, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await FirmataClient.OpenAsync(await _this.ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        }
    }
}
