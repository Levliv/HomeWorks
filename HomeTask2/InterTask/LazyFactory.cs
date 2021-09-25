using System;

namespace InterTask
{
    /// <summary>
    /// Realization of ILazy
    /// </summary>
    public class LazyFactory
    {
        public static ILazy<T> CreateOneThreadLazy<T>(Func<T> supplier)
        {
            return new OneThereadLazyFactory<T>(supplier);
        }
    }

    public class OneThereadLazyFactory<T>: ILazy<T>
    {
        /// <summary>
        /// One Thread LazyFactory
        /// </summary>
        /// <param name="supplier"></param>
        public OneThereadLazyFactory(Func<T> supplier)
        {
            _supplier = supplier;
        }
        private Func<T> _supplier;
        public T _recordedResult { get; set; }
        private bool _isRecorded = false;
        /// <summary>
        /// Lazy in one thread
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
}
