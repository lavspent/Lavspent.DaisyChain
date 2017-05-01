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
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.TaskReactor
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TPredicate"></typeparam>
    public interface IQuery<TResult, TPredicate>
    {
        /// <summary>
        /// 
        /// </summary>
        TPredicate Predicate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        bool Handle(TResult result);

    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TPredicate"></typeparam>
    public class OneTimeQuery<TResult, TPredicate> : IQuery<TResult, TPredicate>
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly OneTimeQuery<TResult, TPredicate> None;

        /// <summary>
        /// 
        /// </summary>
        public TPredicate Predicate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TaskCompletionSource<TResult> TaskCompletionSource { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CancellationToken CancellationToken { get; set; }

        private List<IQuery<TResult, TPredicate>> _queries;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queries"></param>
        public OneTimeQuery(List<IQuery<TResult, TPredicate>> queries)
        {
            _queries = queries;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool Handle(TResult result)
        {
            if (!CancellationToken.IsCancellationRequested)
            {
                // remove ourselves from the queries list
                _queries.Remove(this);

                // complete task with result
                TaskCompletionSource.SetResult(result);

                // stop processing more quries
                return true;
            }

            // continue to next query
            return false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TPredicate"></typeparam>
    public class ObserverQuery<TResult, TPredicate> : IQuery<TResult, TPredicate>
    {
        /// <summary>
        /// 
        /// </summary>
        public TPredicate Predicate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IObserver<TResult> Observer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool Handle(TResult result)
        {
            Observer.OnNext(result);

            // continue to next query
            return true;
        }
    }
}
