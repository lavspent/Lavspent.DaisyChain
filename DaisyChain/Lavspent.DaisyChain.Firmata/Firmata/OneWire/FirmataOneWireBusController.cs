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

using Lavspent.DaisyChain.Firmata.Messages;
using Lavspent.DaisyChain.OneWire;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Firmata
{


    public class FirmataOneWireBusController : IOneWireBusController, IDisposable
    {
        private FirmataClient _firmataClient;

        public FirmataOneWireBusController(FirmataClient firmataClient)
        {
            _firmataClient = firmataClient;
        }

        public async Task<IOneWireBus> OpenOneWireBusAsync(byte? pin = null, bool power = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (pin == null)
                throw new ArgumentOutOfRangeException(nameof(pin), "Pin number cannot be null.");

            // configure the pin for one wire
            var oneWireConfig = new OneWireConfig()
            {
                Pin = (byte)pin,
                Power = power
            };

            await _firmataClient.SendMessageAsync(
                oneWireConfig,
                cancellationToken
                );

            // return bus
            return new FirmataOneWireBus(_firmataClient, (byte)pin);
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
