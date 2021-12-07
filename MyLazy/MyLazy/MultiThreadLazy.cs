using System;

namespace MyLazy
{
    /// <summary>
    /// Multi thread LazyFactory
    /// </summary>
    public class MultiThreadLazy<T> : ILazy<T>
    {
        private Func<T> supplier;
        private readonly object balanceLock = new object();

        /// <summary>
        /// Contains infomation about whether it has been counted
        /// </summary>
        private volatile bool IsRecorded = false;

        /// <summary>
        /// Storing recorded result
        /// </summary>
        private T RecordedResult;

        /// <summary>
        /// Constructor for MutiThread
        /// </summary>
        /// <param name="supplier"></param>
        public MultiThreadLazy(Func<T> supplier) => this.supplier = supplier ?? throw new ArgumentNullException("null ptr is not allowed");

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
                        RecordedResult = supplier();
                        IsRecorded = true;
                        supplier = null;
                    }
                }
            }
            return RecordedResult;
        }
    }
}