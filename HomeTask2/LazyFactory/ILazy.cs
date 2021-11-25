namespace LazyFactory
{
    /// <summary>
    /// Interface for Lazy computation
    /// </summary>
    public interface ILazy<T>
    {
        /// <summary>
        /// Getting the resuts of Func<typeparamref name="T"/>
        /// </summary>
        public T Get();
    }
}
