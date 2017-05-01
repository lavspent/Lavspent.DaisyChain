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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.OneWire
{
    public static class OneWireBusExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public async static Task<IEnumerable<BusBoundOneWireAddress>> WithAddresses(this IOneWireBus _this)
        {
            return (await _this.GetDeviceAddressesAsync().ConfigureAwait(false)).Select(a => new BusBoundOneWireAddress(_this, a));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public async static Task<IEnumerable<BusBoundOneWireAddress>> WithAddresses(this Task<IOneWireBus> _this)
        {
            return await (await _this).WithAddresses();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_this"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static BusBoundOneWireAddress WithAddress(this IOneWireBus _this, OneWireAddress address)
        {
            return new BusBoundOneWireAddress(_this, address);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_this"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public async static Task<BusBoundOneWireAddress> WithAddress(this Task<IOneWireBus> _this, OneWireAddress address)
        {
            return (await _this).WithAddress(address);
        }
    }
}
