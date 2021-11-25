using System;

namespace LazyFactoryNamespace
{
    /// <summary>
    /// Multi thread LazyFactory
    /// </summary>
    public class MultiThreadLazy<T> : ILazy<T>
    {
        private Func<T> _supplier;
        private readonly object balanceLock = new object();

        /// <summary>
        /// Contains infomation about whether it has been counted
        /// </summary>
        public volatile bool IsRecorded = false;

        /// <summary>
        /// Storing a Recocdet afer Lazy init result
        /// </summary>
        public T RecordedResult { get; private set; }

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
            if (!IsRecorded)
            {
                lock (balanceLock)
                {
                    if (!IsRecorded)
                    {
                        RecordedResult = _supplier();
                        IsRecorded = true;
                        _supplier = null;
                    }
                }
            }
            return RecordedResult;
        }
    }
}
