//using System;

//namespace Lavspent.DaisyChain.Misc
//{
//    /// <summary>
//    /// Wrap an observerable and adds support of debouncing.
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    class DebouncedObserverable<T> : IObservable<T> where T : IEquatable<T>
//    {
//        private IObservable<T> _innerObserverable;
//        private T _lastValue;

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="innerObserverable"></param>
//        public DebouncedObserverable(IObservable<T> innerObserverable)
//        {
//            _innerObserverable = innerObserverable;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="observer"></param>
//        /// <returns></returns>
//        public IDisposable Subscribe(IObserver<T> observer)
//        {

//        }
//    }
//}
