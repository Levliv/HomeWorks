using System;
namespace InterTask
{
    /// <summary>
    /// One thread LazyFactory
    /// </summary>
    public class OneThreadLazy<T> : ILazy<T>
    {
        private Func<T> _supplier;
        public T RecordedResult { get; private set; }
        private volatile bool _isRecorded = false;

        /// <summary>
        /// One Thread LazyFactory
        /// </summary>
        /// <param name="supplier"></param>
        public OneThreadLazy(Func<T> supplier)
        {
            if (supplier != null)
            {
                _supplier = supplier;
            }
            else
            {
                throw new ArgumentNullException("Not ptr is not allowed");
            }
        }

        /// <summary>
        /// One thread getter
        /// </summary>
        public T Get()
        {
            if (!_isRecorded)
            {
                RecordedResult = _supplier();
                _supplier = null;
                _isRecorded = true;
            }
            return RecordedResult;
        }
    }
}
