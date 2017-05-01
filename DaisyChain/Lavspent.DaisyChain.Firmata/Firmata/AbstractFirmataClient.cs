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
using Lavspent.TaskReactor;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Firmata
{
    public class FirmataMessageTaskMatcher : ITaskMatcher<IFirmataMessage, FirmataMessageDescriptor>
    {
        public bool IsMatch(IFirmataMessage result, FirmataMessageDescriptor predicate)
        {
            return predicate.IsPartialMatch(result.Command);
        }
    }

    public abstract class AbstractFirmataClient : IDisposable
    {
        private IStream _stream;
        protected IFirmataReader _reader;
        protected IFirmataWriter _writer;
        private CancellationTokenSource _cancellationTokenSource;
        private System.Threading.Tasks.Task _task;
        private const int ReplyTimeout = 100000; // milliseconds

        private SynchronousTaskReactor<IFirmataMessage, FirmataMessageDescriptor> _reactor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public AbstractFirmataClient(IStream stream)
        {
            _stream = stream;
            _cancellationTokenSource = new CancellationTokenSource();
            _reader = new FirmataClientReader(_stream);
            _writer = new FirmataWriter(_stream);
            _reactor = new SynchronousTaskReactor<IFirmataMessage, FirmataMessageDescriptor>(new FirmataMessageTaskMatcher());
        }

        /// <summary>
        /// Start the read Loop
        /// </summary>
        protected void IssueRead()
        {
            // todo: need cancellation
            _reader.ReadAsync(CancellationToken.None).ContinueWith(
                (t) =>
                {
                    _reactor.Post(t.Result);
                    /*if (still connected)*/
                    IssueRead();
                });
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cancellationTokenSource.Cancel();

                // TODO: Wait?
                _cancellationTokenSource.Dispose();
            }
        }

        // TODO: Move somewhere?
        // TODO: Can one use one of those covariant/contravariant in/out instead?
        private class ObserverCaster<T, R> : IObserver<R>
        {
            private IObserver<T> _wrappedObserver;

            public ObserverCaster(IObserver<T> wrappedObserver)
            {
                _wrappedObserver = wrappedObserver;
            }

            public void OnCompleted()
            {
                _wrappedObserver.OnCompleted();
            }

            public void OnError(Exception error)
            {
                _wrappedObserver.OnError(error);
            }

            public void OnNext(R value)
            {
                _wrappedObserver.OnNext((T)(object)value);
            }
        }

        protected Task<IFirmataMessage> QueryResponse(FirmataMessageDescriptor commandInfo, CancellationToken cancellationToken)
        {
            return _reactor.Query(commandInfo, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandInfo"></param>
        /// <param name="observer"></param>
        /// <returns></returns>
        internal IDisposable RegisterResponseObserver(FirmataMessageDescriptor commandInfo, IObserver<IFirmataMessage> observer)
        {
            return _reactor.Subscribe(commandInfo, observer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandInfo"></param>
        /// <param name="observer"></param>
        /// <returns></returns>

        internal IDisposable RegisterResponseObserver<T>(FirmataMessageDescriptor commandInfo, IObserver<T> observer)
        {
            return RegisterResponseObserver(commandInfo, new ObserverCaster<T, IFirmataMessage>(observer));
        }


        internal Task<IFirmataMessage> SendRequestAsync(FirmataMessageDescriptor requestMessageDescriptor,
               FirmataMessageDescriptor responseMessageDescriptor, CancellationToken cancellationToken)
        {
            return SendRequestAsync(requestMessageDescriptor, null, responseMessageDescriptor, cancellationToken);
        }

        internal Task<IFirmataMessage> SendRequestAsync(FirmataMessageDescriptor requestMessageDescriptor, byte[] requestData,
               FirmataMessageDescriptor responseMessageDescriptor, CancellationToken cancellationToken)
        {
            IFirmataMessage message = requestMessageDescriptor.Factory(requestMessageDescriptor, requestMessageDescriptor.Command, requestData);
            return SendRequestAsync(message, responseMessageDescriptor, cancellationToken);
        }

        internal Task<IFirmataMessage> SendRequestAsync(IFirmataMessage message, FirmataMessageDescriptor responseMessageDescriptor, CancellationToken cancellationToken)
        {
            // register response task
            var responseTask = QueryResponse(responseMessageDescriptor, cancellationToken);

            // send a query message
            _writer.WriteAsync(message, cancellationToken);

            // return response task
            return responseTask;
        }

        internal Task SendMessageAsync(IFirmataMessage message, CancellationToken cancellationToken)
        {
            return _writer.WriteAsync(message, cancellationToken);
        }

        internal Task SendMessageAsync(FirmataMessageDescriptor messageDescriptor, CancellationToken cancellationToken)
        {
            return SendMessageAsync(messageDescriptor, null, cancellationToken);
        }

        internal Task SendMessageAsync(FirmataMessageDescriptor messageDescriptor, byte[] messageData, CancellationToken cancellationToken)
        {
            IFirmataMessage message = messageDescriptor.Factory(messageDescriptor, messageDescriptor.Command, messageData);
            return SendMessageAsync(message, cancellationToken);
        }
    }

}
