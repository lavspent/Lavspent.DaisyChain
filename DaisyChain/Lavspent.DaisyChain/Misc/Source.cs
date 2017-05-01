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

using Lavspent.Backport;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lavspent.DaisyChain.Misc
{
    /// <summary>
    /// Simple staight forward implementation of IObservable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Source<T> : IObservable<T>//, IDisposable
    {
        // TODO: Use some kind of concurrent collection?
        private List<IObserver<T>> _observers;
        private object _lock = new object();

        /// <summary>
        /// 
        /// </summary>
        public Source()
        {
            _lock = new object();
            _observers = new List<IObserver<T>>();
        }

        /// <summary>
        /// Subscribe to this source.
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            lock (_lock)
            {
                if (_observers.Contains(observer))
                    throw new Exception("Observer already subscribing to this source.");

                _observers.Add(observer);
            }

            return new Unsubscriber(_observers, observer, _lock);
        }

        /// <summary>
        /// 
        /// </summary>
        private class Unsubscriber : IDisposable
        {
            private List<IObserver<T>> _observers;
            private IObserver<T> _observer;
            private object _lock;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="observers"></param>
            /// <param name="observer"></param>
            /// <param name="lock"></param>
            public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer, object @lock)
            {
                _observers = observers;
                _observer = observer;
                _lock = @lock;
            }

            /// <summary>
            /// 
            /// </summary>
            /// 
            private bool _disposed = false;

            protected virtual void Dispose(bool disposing)
            {
                lock (_lock)
                {
                    if (!_disposed)
                    {
                        if (_observer != null)
                            _observers.Remove(_observer);
                    }
                }

                _disposed = true;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(true);
            }

            ~Unsubscriber()
            {
                Dispose(false);
            }
        }

        /// <summary>
        /// Send new value to observers of this source/observable.
        /// </summary>
        /// <param name="value"></param>
        public void Next(T value)
        {
            ParallelForEach((o) => o.OnNext(value));
        }

        /// <summary>
        /// Tell observers that this source/observable is comeplete.
        /// </summary>
        public void Completed()
        {
            ParallelForEach((o) => o.OnCompleted());
        }

        /// <summary>
        /// Send an error to observers of this source/observable.
        /// </summary>
        /// <param name="error"></param>
        public void Error(Exception error)
        {
            ParallelForEach((o) => o.OnError(error));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        private void ParallelForEach(Action<IObserver<T>> action)
        {
            List<IObserver<T>> observers;

            lock (_lock)
            {
                observers = _observers.ToList();
            }

            Parallel.ForEach(observers, (o) => action(o));
        }


        //#region IDisposable Support
        //private bool _disposed = false; // To detect redundant calls

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!_disposed)
        //    {
        //        if (disposing)
        //        {

        //        }

        //        _disposed = true;
        //    }
        //}

        //~Source()
        //{
        //    Dispose(false);
        //}

        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}
        //#endregion
    }
}
