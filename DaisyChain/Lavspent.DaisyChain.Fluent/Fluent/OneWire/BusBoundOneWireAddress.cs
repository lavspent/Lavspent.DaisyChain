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

namespace Lavspent.DaisyChain.OneWire
{
    /// <summary>
    /// A OneWireAddress bound to a specific OneWireBus.
    /// </summary>
    public class BusBoundOneWireAddress : OneWireAddress
    {
        /// <summary>
        /// OneWireBus which this address is bound to.
        /// </summary>
        private IOneWireBus _oneWireBus;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="address"></param>
        public BusBoundOneWireAddress(IOneWireBus bus, OneWireAddress address)
            : base(address)
        {
            _oneWireBus = bus;
        }

        ///// <summary>
        ///// Open device of type T on this address.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="_this"></param>
        ///// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        ///// <returns></returns>
        //public Task<T> OpenDeviceAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
        //    where T : class, IOneWireDevice
        //{
        //    return _oneWireBus.OpenDeviceAsync<T>(this, null, cancellationToken);
        //}

        ///// <summary>
        ///// Open device of type T using provided or default type factory on this address.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="_this"></param>
        ///// <param name="factory"></param>
        ///// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        ///// <returns></returns>
        //public Task<T> OpenDeviceAsync<T>(IOneWireDeviceFactory<T> factory, CancellationToken cancellationToken = default(CancellationToken))
        //    where T : class, IOneWireDevice
        //{
        //    return _oneWireBus.OpenDeviceAsync(this, factory, cancellationToken);
        //}
    }
}
