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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Firmata
{
    internal class FirmataOneWireBus : IOneWireBus, IDisposable
    {
        private FirmataClient _firmataClient;
        private byte _pin;


        public FirmataOneWireBus(FirmataClient firmataClient, byte pin)
        {
            _firmataClient = firmataClient;
            _pin = pin;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public Task ResetAsync(CancellationToken cancellationToken)
        {
            var message = new OneWireCommand();
            message.Pin = this._pin;
            message.Commands = OneWireCommand.CommandFlags.Reset;
            return _firmataClient.SendMessageAsync(message, cancellationToken);
        }

        public Task SkipRomAsync(CancellationToken cancellationToken)
        {
            var message = new OneWireCommand();
            message.Pin = this._pin;
            message.Commands = OneWireCommand.CommandFlags.Skip;
            return _firmataClient.SendMessageAsync(message, cancellationToken);
        }

        public Task MatchRomAsync(byte[] rom, CancellationToken cancellationToken)
        {
            var message = new OneWireCommand();
            message.Pin = this._pin;
            message.Commands = OneWireCommand.CommandFlags.Select;
            message.Address = rom;
            return _firmataClient.SendMessageAsync(message, cancellationToken);
        }

        public async Task<IEnumerable<OneWireAddress>> GetDeviceAddressesAsync(CancellationToken cancellationToken)
        {
            var request = new OneWireSearch();
            request.Pin = this._pin;
            request.SearchAlarms = false;

            var reply = await _firmataClient.SendRequestAsync(
                request,
                OneWireSearchReply.MessageDescriptor,
                default(CancellationToken)
                );


            return ((OneWireSearchReply)reply).Addresses;
        }

        public Task<byte> ReadAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task ReadAsync(byte[] buffer, int length, CancellationToken cancellationToken)
        {
            // TODO: Clean up!

            var request = new OneWireCommand();
            request.Pin = this._pin;
            request.Commands = OneWireCommand.CommandFlags.Read;
            request.BytesToRead = (ushort)length;
            request.CorrelationId = 0;

            var reply = await _firmataClient.SendRequestAsync(
              request,
              OneWireReadReply.MessageDescriptor,
              default(CancellationToken)
              );

            // todo: fix
            var c = ((OneWireReadReply)reply).Read;

            c.CopyTo(buffer, 0);
        }

        public Task<byte> ReadBitAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task WriteBitAsync(byte data, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task WriteAsync(byte[] data, CancellationToken cancellationToken)
        {
            var message = new OneWireCommand();
            message.Pin = this._pin;
            message.Commands = OneWireCommand.CommandFlags.Write;
            message.DataToWrite = data;
            return _firmataClient.SendMessageAsync(message, cancellationToken);
        }
    }
}
