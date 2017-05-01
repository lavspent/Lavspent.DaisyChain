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

using Lavspent.AsyncInline;
using Lavspent.Backport;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.TaskReactor
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TPredicate"></typeparam>
    public class AsynchronousTaskReactor<TResult, TPredicate> : AbstractTaskReactor<TResult, TPredicate>, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        private Task _task;

        /// <summary>
        /// 
        /// </summary>
        protected List<TResult> _results = new List<TResult>();

        /// <summary>
        /// 
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;


        /// <summary>
        /// 
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matcher"></param>
        public AsynchronousTaskReactor(ITaskMatcher<TResult, TPredicate> matcher)
            : base(matcher)
        {

            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            _task = new Task(() => Run(cancellationToken), cancellationToken);
            _task.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        private void Run(CancellationToken cancellationToken)
        {
            List<TResult> newResults = new List<TResult>();

            while (!cancellationToken.IsCancellationRequested)
            {
                lock (_results)
                {
                    // wait until we got any results
                    while (_results.Count == 0 && !cancellationToken.IsCancellationRequested)
                        Monitor.Wait(_results);

                    if (cancellationToken.IsCancellationRequested)
                        return;

                    // grab them
                    newResults.AddRange(_results);
                    _results.Clear();
                }

                // handle the results
                lock (_queries)
                {
                    newResults.ForEach(HandleResult);
                }

                newResults.Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public override void Post(TResult result)
        {
            lock (_results)
            {
                _results.Add(result);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _cancellationTokenSource.Cancel();
                    _task.WaitInline();
                }

                disposed = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
    }
}
