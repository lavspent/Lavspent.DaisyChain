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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.OneWire
{
    public static class OneWireBusExtensions
    {
        /// <summary>
        /// Convenient when sending only one byte of data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Task WriteAsync(this IOneWireBus _this, byte data, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _this.WriteAsync(new byte[] { data }, cancellationToken);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="_this"></param>
        ///// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        ///// <returns></returns>
        //public static Task<T> OpenDeviceAsync<T>(this IOneWireBus _this, OneWireAddress address, CancellationToken cancellationToken = default(CancellationToken))
        //    where T : class, IOneWireDevice
        //{
        //    return OpenDeviceAsync<T>(_this, address, null, cancellationToken);
        //}

        ///// <summary>
        ///// Open device of type T using provided or default type factory.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="_this"></param>
        ///// <param name="factory"></param>
        ///// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        ///// <returns></returns>
        //public static Task<T> OpenDeviceAsync<T>(this IOneWireBus _this, OneWireAddress address, IOneWireDeviceFactory<T> factory, CancellationToken cancellationToken = default(CancellationToken))
        //    where T : class, IOneWireDevice
        //{
        //    // get or try create factory
        //    factory = factory ?? OneWireDeviceFactoryAttribute.GetFactory<T>();

        //    if (factory == null)
        //    {
        //        throw new Exception("No factory specified. Must be provided either as an argument or an attribute on the device class.");
        //    }

        //    return factory.OpenDeviceAsync(_this, address);
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="_this"></param>
        ///// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        ///// <returns></returns>
        //public static async Task<T> OpenDeviceAsync<T>(this Task<IOneWireBus> _this, OneWireAddress address, CancellationToken cancellationToken = default(CancellationToken))
        //    where T : class, IOneWireDevice
        //{
        //    return await (await _this.ConfigureAwait(false)).OpenDeviceAsync<T>(address, null, cancellationToken).ConfigureAwait(false);
        //}

        //// <summary>
        //// Open device of type T using provided or default type factory.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="_this"></param>
        ///// <param name="factory"></param>
        ///// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        ///// <returns></returns>
        //public static async Task<T> OpenDeviceAsync<T>(this Task<IOneWireBus> _this, OneWireAddress address, IOneWireDeviceFactory<T> factory, CancellationToken cancellationToken = default(CancellationToken))
        //        where T : class, IOneWireDevice
        //{
        //    return await (await _this.ConfigureAwait(false)).OpenDeviceAsync<T>(address, factory, cancellationToken).ConfigureAwait(false);
        //}
    }
}
