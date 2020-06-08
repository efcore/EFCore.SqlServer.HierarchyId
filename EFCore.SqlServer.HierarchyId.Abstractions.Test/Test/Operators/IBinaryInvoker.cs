using System;
using System.Collections.Generic;

namespace Xunit.Operators
{
    /// <summary>
    /// The interface used by operator tests
    /// </summary>
    public interface IBinaryInvoker
    {
        /// <summary>
        /// Calls the binaryexpression with the left and right and returns the result
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        TResult Invoke<TLeft, TRight, TResult>(TLeft left, TRight right);

        /// <summary>
        /// Calls the binaryexpression with the left and every element in right and invokes an action
        /// while iterating
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="action"></param>
        void InvokeAll<TLeft, TRight, TResult>(TLeft left, IEnumerable<TRight> right, Action<long, TLeft, TRight, TResult> action);
    }
}
