using System;

namespace InterTask
{
    /// <summary>
    /// Lazy factory
    /// </summary>  
    public class LazyFactory
    {
        /// <summary>
        /// Creates one thread Lazy implementing interface ILazy
        /// </summary>
        public static ILazy<T> CreateOneThreadLazy<T>(Func<T> supplier)
            => new OneThreadLazy<T>(supplier);

        /// <summary>
        /// Creates multi thread Lazy implementing interface ILazy
        /// </summary>
        public static ILazy<T> CreateMultiThreadLazy<T>(Func<T> supplier)
            => new MultiThreadLazy<T>(supplier);
    }
}
