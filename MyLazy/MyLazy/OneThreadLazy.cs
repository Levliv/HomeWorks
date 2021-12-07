using System;
namespace MyLazy
{
    /// <summary>
    /// One thread LazyFactory
    /// </summary>
    public class OneThreadLazy<T> : ILazy<T>
    {
        private Func<T> supplier;

        /// <summary>
        /// Contains infomation about whether it has been counted 
        /// </summary>
        private bool isRecorded = false;

        /// <summary>
        /// Storing a Recocdet afer Lazy init result
        /// </summary>
        private T recordedResult;

        /// <summary>
        /// One Thread LazyFactory
        /// </summary>
        /// <param name="supplier"></param>
        public OneThreadLazy(Func<T> supplier) => this.supplier = supplier ?? throw new ArgumentNullException("Not ptr is not allowed");

        /// <summary>
        /// One thread getter
        /// </summary>
        public T Get()
        {
            if (!isRecorded)
            {
                recordedResult = supplier();
                supplier = null;
                isRecorded = true;
            }
            return recordedResult;
        }
    }
}