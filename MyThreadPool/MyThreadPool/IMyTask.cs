namespace MyThreadPool;

/// <summary>
/// Task representation.
/// </summary>
public interface IMyTask<out TResult>
{
    /// <summary>
    /// Gets flag representing whether the computation is completed.
    /// </summary>
    bool IsCompleted { get; }

    /// <summary>
    /// Gets result value.
    /// </summary>
    TResult Result { get; }

    /// <summary>
    /// Continues computation with TResult.
    /// </summary>
    /// <typeparam name="TNewResult"> Result after computation. </typeparam>
    /// <param name="func"> Applies to TResult to get TNewResult. </param>
    /// <returns> Result of applying func. </returns>
    IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func);
}
