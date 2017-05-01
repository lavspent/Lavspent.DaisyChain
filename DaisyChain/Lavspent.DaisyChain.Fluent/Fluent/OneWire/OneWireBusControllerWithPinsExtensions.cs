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

//using System.ComponentModel;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Lavspent.DaisyChain.OneWire
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    [EditorBrowsable(EditorBrowsableState.Never)]
//    public static class OneWireBusControllerWithPinsExtensions
//    {
//        /// <summary>
//        /// Bind this OneWireBusController to a specific pin.
//        /// <param name="_this"></param>
//        /// <param name="pin"></param>
//        /// <returns></returns>
//        public static IOneWireBusController OnPin(this IOneWireBusControllerWithPins _this, byte pin)
//        {
//            return new PinBoundOneWireBusController(_this, pin);
//        }

//        /// <summary>
//        /// Bind this OneWireBusController to a specific pin.
//        /// </summary>
//        /// <param name="_this"></param>
//        /// <param name="pin">The pin number</param>
//        /// <returns></returns>
//        public async static Task<IOneWireBusController> OnPin(this Task<IOneWireBusControllerWithPins> _this, byte pin)
//        {
//            return (await _this.ConfigureAwait(false)).OnPin(pin);
//        }
//    }
//}
