///*
//    Copyright(c) 2017 Petter Labråten/LAVSPENT.NO. All rights reserved.

//    The MIT License(MIT)

//    Permission is hereby granted, free of charge, to any person obtaining a
//    copy of this software and associated documentation files (the "Software"),
//    to deal in the Software without restriction, including without limitation
//    the rights to use, copy, modify, merge, publish, distribute, sublicense,
//    and/or sell copies of the Software, and to permit persons to whom the
//    Software is furnished to do so, subject to the following conditions:

//    The above copyright notice and this permission notice shall be included in
//    all copies or substantial portions of the Software.

//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
//    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
//    DEALINGS IN THE SOFTWARE.
//*/

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Lavspent.DaisyChain.Misc
//{
//    /// <summary>
//    /// Debounce implementation of IObservable.
//    /// Only changes in values are reported, and only at given intervals.
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    public class DebounceSource<T> : IObservable<T> where T : IEquatable<T>
//    {
//        private List<IObserver<T>> _observers;
//        private object _lock = new object();
//        private DateTime _lastValueChange = DateTime.MinValue;

//        private CancellationTokenSource _cancellationTokenSource;
//        private CancellationToken _cancellationToken;

//        private Timer _timer;


//        /// <summary>
//        /// 
//        /// </summary>
//        public static TimeSpan DebounceTimeout { get; set; } = TimeSpan.Zero;


//        /// <summary>
//        /// 
//        /// </summary>
//        public DebounceSource()
//            : this(TimeSpan.Zero)
//        {
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public DebounceSource(TimeSpan debouceTimeout)
//        {
//            _lock = new object();
//            _observers = new List<IObserver<T>>();
//            DebounceTimeout = debouceTimeout;
//        }

//        /// <summary>
//        /// Subscribe to this source.
//        /// </summary>
//        /// <param name="observer"></param>
//        /// <returns></returns>
//        public IDisposable Subscribe(IObserver<T> observer)
//        {
//            lock (_lock)
//            {
//                if (_observers.Contains(observer))
//                    throw new Exception("Observer already subscribing to this source.");

//                _observers.Add(observer);
//            }

//            return new Unsubscriber(_observers, observer, _lock);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        private class Unsubscriber : IDisposable
//        {
//            private List<IObserver<T>> _observers;
//            private IObserver<T> _observer;
//            private object _lock;

//            /// <summary>
//            /// 
//            /// </summary>
//            /// <param name="observers"></param>
//            /// <param name="observer"></param>
//            /// <param name="lock"></param>
//            public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer, object @lock)
//            {
//                this._observers = observers;
//                this._observer = observer;
//                this._lock = @lock;
//            }

//            /// <summary>
//            /// 
//            /// </summary>
//            public void Dispose()
//            {
//                lock (_lock)
//                {
//                    if (_observer != null)
//                        _observers.Remove(_observer);
//                }
//            }
//        }

//        /// <summary>
//        /// Send new value to observers of this source/observable.
//        /// </summary>
//        /// <param name="value"></param>
//        public void Next(T value)
//        {
//            lock (_lock)
//            {
//                // stop timer
//                _timer.Change(Timeout.Infinite, Timeout.Infinite);

//                DateTime now = DateTime.UtcNow;

//                if (_lastValueChange

//                    ParallelForEach((o) => o.OnNext(value));
//            }
//        }

//        /// <summary>
//        /// Tell observers that this source/observable is comeplete.
//        /// </summary>
//        public void Completed()
//        {
//            ParallelForEach((o) => o.OnCompleted());
//        }

//        /// <summary>
//        /// Send an error to observers of this source/observable.
//        /// </summary>
//        /// <param name="error"></param>
//        public void Error(Exception error)
//        {
//            ParallelForEach((o) => o.OnError(error));
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="action"></param>
//        private void ParallelForEach(Action<IObserver<T>> action)
//        {
//            List<IObserver<T>> observers;

//            lock (_lock)
//            {
//                observers = _observers.ToList();
//            }

//            Parallel.ForEach(observers, (o) => action(o));
//        }
//    }
//}
