using System;

namespace InterTask
{
    /// <summary>
    /// Lazy factory
    /// </summary>
    public class LazyFactory
    {
        public static ILazy<T> CreateOneThreadLazy<T>(Func<T> supplier)
        {
            return new OneThereadLazyFactory<T>(supplier);
        }
        public static ILazy<T> CreateMultiThreadLazy<T>(Func<T> supplier)
        {
            return new MultiThreadLazyFactory<T>(supplier);
        }
    }

    /// <summary>
    /// One thread LazyFactory
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OneThereadLazyFactory<T>: ILazy<T>
    {
        private Func<T> _supplier;
        public T _recordedResult { get; set; }
        private bool _isRecorded = false;

        /// <summary>
        /// One Thread LazyFactory
        /// </summary>
        /// <param name="supplier"></param>
        public OneThereadLazyFactory(Func<T> supplier)
        {
            _supplier = supplier;
        }

        /// <summary>
        /// One thread getter
        /// </summary>
        /// <returns></returns>
        public T Get() {
            if (!_isRecorded)
            {
                _recordedResult = _supplier();
                _isRecorded = true;
            }
            return _recordedResult;
        }
    }
    /// <summary>
    /// Multi thread LazyFactory
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MultiThreadLazyFactory<T> : ILazy<T>
    {
        private Func<T> _supplier;
        public T _recordedResult { get; set; }
        private bool _isRecorded = false;

        /// <summary>
        /// Constructor for MutiThread
        /// </summary>
        /// <param name="supplier"></param>
        public MultiThreadLazyFactory(Func<T> supplier)
        {
            _supplier = supplier;
        }

        /// <summary>
        /// Multi thread getter
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            _recordedResult = _supplier();
            _isRecorded = true;
            return _recordedResult;
        }
    }
}
