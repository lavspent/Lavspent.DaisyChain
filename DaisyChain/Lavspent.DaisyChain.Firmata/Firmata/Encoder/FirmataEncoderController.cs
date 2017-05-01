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

using Lavspent.DaisyChain.Devices;
using Lavspent.DaisyChain.Firmata.Messages;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Firmata.Encoder
{
    internal class FirmataEncoderController : IEncoderController
    {
        private FirmataClient _firmataClient;
        private Dictionary<int, FirmataEncoder> _encoders;
        public FirmataEncoderController(FirmataClient firmataClient)
        {
            _firmataClient = firmataClient;
            _encoders = new Dictionary<int, Encoder.FirmataEncoder>();
        }

        public async Task<IEncoder> OpenEncoderAsync(byte encoderNumber, byte pinA, byte pinB, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (encoderNumber < 0 || encoderNumber > 4)
                throw new ArgumentException("Invalid encoderNumber.");

            if (_encoders.ContainsKey(encoderNumber))
                throw new ArgumentException("Encoder already open.");

            var encoderAttach = new EncoderAttach()
            {
                EncoderNumber = encoderNumber,
                PinA = pinA,
                PinB = pinB
            };

            // doesn't return anything, assume ok
            await _firmataClient.SendMessageAsync(encoderAttach, cancellationToken);

            // store and return encoder
            _encoders[encoderNumber] = new FirmataEncoder(this, encoderNumber, pinA, pinB);
            return _encoders[encoderNumber];
        }

        internal void Close(FirmataEncoder encoder)
        {
            _encoders.Remove(encoder.EncoderNumber);
        }

        internal FirmataClient FirmataClient
        {
            get
            {
                return _firmataClient;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        internal IDisposable RegisterEncoderDataObserver(IObserver<EncoderData> observer)
        {
            return _firmataClient.RegisterResponseObserver(EncoderData.MessageDescriptor, observer);
        }
    }
}
