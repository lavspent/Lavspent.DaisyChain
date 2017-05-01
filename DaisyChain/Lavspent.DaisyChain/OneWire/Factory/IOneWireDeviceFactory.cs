///*
//    Copyright(c) 2017 Petter Labråten/LAVSPENT.NO. All rights reserved.

//    The MIT License(MIT)

//    Permission is hereby granted, free of charge, to any person obtaining a
//    copy of this software and associated documentation files (the "Software"),
//    to deal in the Software without restriction, including without limitation
//    the rights to use, copy, modify, merge, publish, distribute, sublicense,
//    and/or sell copies of the Software, and to permit persons to whom the
//    Software is furnished to do so, subject to the following conditions:

//    The above copyright notice and this permission notice shall be included in
//    all copies or substantial portions of the Software.

//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
//    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
//    DEALINGS IN THE SOFTWARE.
//*/

//using System.Threading;
//using System.Threading.Tasks;

//namespace Lavspent.DaisyChain.OneWire
//{
//    ///// <summary>
//    ///// Delegate for OneWireDevice factory methods
//    ///// </summary>
//    ///// <typeparam name="T"></typeparam>
//    ///// <returns></returns>
//    //public delegate Task<T> OneWireDeviceFactoryDelegate<T>() where T : class, IOneWireDevice;

//    //public interface IOneWireDeviceFactoryProvider<T> where T: class, IOneWireDevice
//    //{
//    //    Task<T> OpenDeviceFactoryAsync(IOneWireBus oneWireBus, OneWireAddress address, CancellationToken cancellationToken = default(CancellationToken));
//    //}

//    /// <summary>
//    /// Interface for OneWireDevice factory classes.
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    public interface IOneWireDeviceFactory<T> where T : class, IOneWireDevice
//    {
//        // There should only be one method here.

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="oneWireBus"></param>
//        /// <param name="address"></param>
//        /// <param name="cancellationToken"></param>
//        /// <returns></returns>
//        Task<T> OpenDeviceAsync(IOneWireBus oneWireBus, OneWireDeviceArguments arguments, CancellationToken cancellationToken = default(CancellationToken));
//    }
//}
