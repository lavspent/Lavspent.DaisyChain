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

//using Lavspent.DaisyChain.OneWire;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Lavspent.DaisyChain.OneWire
//{
//    public class PinBoundOneWireBusController : IOneWireBusController
//    {
//        private IOneWireBusControllerWithPins _controller;
//        private byte _pin;

//        public PinBoundOneWireBusController(IOneWireBusControllerWithPins controller, byte pin)
//        {
//            _controller = controller;
//            _pin = pin;
//        }

//        public Task<IOneWireBus> OpenOneWireBusAsync(bool power = true, CancellationToken cancellationToken = default(CancellationToken))
//        {
//            return _controller.OpenOneWireBusAsync(_pin, power, cancellationToken);
//        }
//    }
//}
