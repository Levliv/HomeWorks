using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterTask
{
    /// <summary>
    /// Interface ILazy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILazy<T>
    {
    /// <summary>
    /// Getting the resuts of Func<typeparamref name="T"/>
    /// </summary>
    /// <returns>Counting result</returns>
        T Get();
    }
}
