using System;
namespace LazyFactory
{
    /// <summary>
    /// One thread LazyFactory
    /// </summary>
    public class OneThreadLazy<T> : ILazy<T>
    {
        private Func<T> _supplier;
        /// <summary>
        /// Contains infomation about whether it has been counted 
        /// </summary>
        public bool IsRecorded = false;

        /// <summary>
        /// Storing a Recocdet afer Lazy init result
        /// </summary>
        public T RecordedResult { get; private set; }

        /// <summary>
        /// One Thread LazyFactory
        /// </summary>
        /// <param name="supplier"></param>
        public OneThreadLazy(Func<T> supplier) => _supplier = supplier ?? throw new ArgumentNullException("Not ptr is not allowed");

        /// <summary>
        /// One thread getter
        /// </summary>
        public T Get()
        {
            if (!IsRecorded)
            {
                RecordedResult = _supplier();
                _supplier = null;
                IsRecorded = true;
            }
            return RecordedResult;
        }
    }
}
