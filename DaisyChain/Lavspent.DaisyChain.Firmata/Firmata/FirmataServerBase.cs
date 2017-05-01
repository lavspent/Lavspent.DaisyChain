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
using Lavspent.DaisyChain.Stream;
using Lavspent.RuntimeTypeDispatcher;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Firmata
{

    public class FirmataServerBase : IFirmataServer
    {
        private IStream _stream;
        private IFirmataReader _reader;
        private IFirmataWriter _writer;
        private volatile bool _stop = false;

        private AsyncRuntimeTypeDispatcher<IFirmataMessage> _runtimeTypeDispatcher;


        public FirmataServerBase()
        {
            // setup message handlers
            _runtimeTypeDispatcher = new AsyncRuntimeTypeDispatcher<IFirmataMessage>();
        }

        protected void RegisterHandler<S>(AsyncRuntimeTypeDispatcher<IFirmataMessage>.HandlerDelegate<S> handler) where S : class, IFirmataMessage
        {
            _runtimeTypeDispatcher.RegisterHandler(handler);
        }

        public async Task ConnectAsync(IStream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            //if (!stream.CanRead || !stream.CanWrite)
            //    throw new ArgumentException("Need a stream that supports both reading and writing.", nameof(stream));
            _stream = stream;
            _reader = new FirmataServerReader(_stream);
            _writer = new FirmataWriter(_stream);


            // todo: implement cancellation
            CancellationTokenSource cts = new CancellationTokenSource();

            while (!_stop)
            {
                try
                {
                    IFirmataMessage message = await _reader.ReadAsync();
                    await HandleMessageAsync(message, cts.Token);
                }
                catch (Exception)
                {
                    // 
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Disconnect()
        {
            _stop = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        private Task HandleMessageAsync(IFirmataMessage message, CancellationToken cancellationToken)
        {
            var messageHandler = _runtimeTypeDispatcher.GetHandler(message);
            if (messageHandler != null)
            {
                System.Threading.Tasks.Task task = messageHandler(message, cancellationToken);
                if (task == null)
                    throw new Exception("The result from the message handler delegate must be a valid Task.");

                return task;
            }
            else
                throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        protected Task WriteAsync(IFirmataMessage message, CancellationToken cancellationToken)
        {
            return _writer.WriteAsync(message, cancellationToken);
        }
    }
}
