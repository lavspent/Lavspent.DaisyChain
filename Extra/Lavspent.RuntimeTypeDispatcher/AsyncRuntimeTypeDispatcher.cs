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
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.RuntimeTypeDispatcher
{
    // todo: get a better name?
    // todo: can i be done i a simple manner?
    public class AsyncRuntimeTypeDispatcher<T> where T : class
    {
        // todo: does s have to be subtype of t?
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="message"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public delegate Task HandlerDelegate<S>(S message, CancellationToken cancellationToken) where S : class, T;
        private Dictionary<TypeInfo, HandlerDelegate<T>> _handlers = new Dictionary<TypeInfo, HandlerDelegate<T>>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="handler"></param>
        public void RegisterHandler<S>(HandlerDelegate<S> handler) where S : class, T
        {
            _handlers.Add(typeof(S).GetTypeInfo(), UpcastHandler(handler));
        }

        private HandlerDelegate<T> UpcastHandler<S>(HandlerDelegate<S> handler) where S : class, T
            => (t, ct) => handler((S)t, ct);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public HandlerDelegate<T> GetHandler(T message)
        {
            TypeInfo messageType = message.GetType().GetTypeInfo();
            HandlerDelegate<T> handler = null;

            // we got an exact match?
            if (_handlers.ContainsKey(messageType))
            {
                handler = _handlers[messageType];
            }

            // 
            if (handler == null)
            {
                var matchingType = _handlers.Keys.SingleOrDefault(k => k.IsAssignableFrom(messageType));
                if (matchingType != null)
                {
                    handler = _handlers[matchingType];

                    // todo: persisting the machign handler. Is this good practice?
                    _handlers.Add(matchingType, handler);
                }
            }

            if (handler != null)
            {
                // wrap and return
                return (m, ct) =>
                {
                    return (Task)((Delegate)handler).DynamicInvoke(m, ct);
                };
            }

            // no match
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task InvokeAsync(T message, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetHandler(message)(message, cancellationToken);
        }
    }
}
