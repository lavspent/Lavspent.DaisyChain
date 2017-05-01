/*
    Copyright(c) 2017 Petter Labråten. All rights reserved.

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
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.TaskReactor
{
    // TODO: messy

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TPredicate"></typeparam>
    public abstract class AbstractTaskReactor<TResult, TPredicate> : ITaskReactor<TResult, TPredicate>
    {
        /// <summary>
        /// 
        /// </summary>
        protected List<IQuery<TResult, TPredicate>> _queries = new List<IQuery<TResult, TPredicate>>();

        /// <summary>
        /// 
        /// </summary>
        protected ITaskMatcher<TResult, TPredicate> _matcher;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matcher"></param>
        public AbstractTaskReactor(ITaskMatcher<TResult, TPredicate> matcher)
        {
            _matcher = matcher;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public abstract void Post(TResult result);

        /// <summary>
        /// Called when a query is cancelled.
        /// </summary>
        /// <param name="state"></param>
        private void OneTimeQueryCancelled(object state)
        {
            var query = (OneTimeQuery<TResult, TPredicate>)state;

            lock (_queries)
            {
                _queries.Remove(query);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task<TResult> Query(TPredicate predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = new OneTimeQuery<TResult, TPredicate>(this._queries);
            query.Predicate = predicate;
            query.CancellationToken = cancellationToken;
            query.TaskCompletionSource = new TaskCompletionSource<TResult>();

            cancellationToken.Register(OneTimeQueryCancelled, query);

            lock (_queries)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    _queries.Add(query);
                }
            }

            return query.TaskCompletionSource.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        protected void HandleResult(TResult result)
        {
#if DEBUG
            Debug.WriteLine(result.ToString());

            if (!Monitor.IsEntered(_queries))
                throw new InvalidOperationException($"Expected a lock on {nameof(_queries)}");
#endif

            for (int i = 0; i < _queries.Count; i++)
            {
                var query = _queries[i];
                if (_matcher.IsMatch(result, query.Predicate))
                {
                    bool @break = query.Handle(result);
                    if (!@break)
                        break;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="observer"></param>
        /// <returns></returns>
        public IDisposable Subscribe(TPredicate predicate, IObserver<TResult> observer)
        {
            var queryInfo = new ObserverQuery<TResult, TPredicate>();
            queryInfo.Predicate = predicate;
            queryInfo.Observer = observer;

            lock (_queries)
            {
                _queries.Insert(0, queryInfo);
            }

            return new Unsubscriber(this, queryInfo);
        }

        /// <summary>
        /// 
        /// </summary>
        private class Unsubscriber : IDisposable
        {
            private AbstractTaskReactor<TResult, TPredicate> _taskReactor;
            private ObserverQuery<TResult, TPredicate> _observerInfo;

            public Unsubscriber(
                AbstractTaskReactor<TResult, TPredicate> taskReactor,
                ObserverQuery<TResult, TPredicate> observerInfo)
            {
                this._taskReactor = taskReactor;
                this._observerInfo = observerInfo;
            }

            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                    }

                    lock (_taskReactor._queries)
                    {
                        if (_observerInfo != null && _taskReactor._queries.Contains(_observerInfo))
                            _taskReactor._queries.Remove(_observerInfo);
                    }

                    disposedValue = true;
                }
            }

            ~Unsubscriber()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }
    }
}
