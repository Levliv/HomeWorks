using System;

namespace LazyFactoryNamespace
{
    /// <summary>
    /// Multi thread LazyFactory
    /// </summary>
    public class MultiThreadLazy<T> : ILazy<T>
    {
        private Func<T> _supplier;
        public T RecordedResult { get; private set; }
        private volatile bool _isRecorded = false;
        private readonly object balanceLock = new object();

        /// <summary>
        /// Constructor for MutiThread
        /// </summary>
        /// <param name="supplier"></param>
        public MultiThreadLazy(Func<T> supplier) => _supplier = supplier ?? throw new ArgumentNullException("null ptr is not allowed");

        /// <summary>
        /// Multi thread getter
        /// </summary>
        public T Get()
        {
            if (!_isRecorded)
            {
                lock (balanceLock)
                {
                    if (!_isRecorded)
                    {
                        RecordedResult = _supplier();
                        _isRecorded = true;
                        _supplier = null;
                    }
                }
            }
            return RecordedResult;
        }
    }
}
