using System;
using System.Collections.Generic;

namespace Xunit.Operators
{
    /// <summary>
    /// The interface used by operator tests
    /// </summary>
    public interface IBooleanOperatorAsserter : IBinaryInvoker
    {
        /// <summary>
        /// returns the operator as a string (e.g. "==", "!=", "<", etc)
        /// </summary>
        string Operator { get; }

        /// <summary>
        /// Calls the binaryexpression with the left and right and returns the result as a bool
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        bool Invoke<TLeft, TRight>(TLeft left, TRight right);

        /// <summary>
        /// Calls the binaryexpression with the left and every element in right and invokes an action
        /// while iterating, with a boolean as the result
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="action"></param>
        void InvokeAll<TLeft, TRight>(TLeft left, IEnumerable<TRight> right, Action<long, TLeft, TRight, bool> action);

        /// <summary>
        /// Checks the left value against the right and asserts true
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        void True<TLeft, TRight>(TLeft left, TRight right);

        /// <summary>
        /// Checks the left value against each element in the right. If any are not true,
        /// the criteria is not satisfied
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        void TrueAll<TLeft, TRight>(TLeft left, IEnumerable<TRight> right);

        /// <summary>
        /// Checks the left value against the right and asserts false
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        void False<TLeft, TRight>(TLeft left, TRight right);

        /// <summary>
        /// Checks the left value against each element in the right. If any are not false,
        /// the criteria is not satisfied
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        void FalseAll<TLeft, TRight>(TLeft left, IEnumerable<TRight> right);
        
    }
}
