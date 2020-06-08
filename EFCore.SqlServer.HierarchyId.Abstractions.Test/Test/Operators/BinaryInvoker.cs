using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Xunit.Operators
{
    internal class BinaryInvoker : IBinaryInvoker
    {
        private readonly BinaryExpressionCache expressionCache;

        public TResult Invoke<TLeft, TRight, TResult>(TLeft left, TRight right)
            => expressionCache.Get<TLeft, TRight, TResult>()(left, right);

        public void InvokeAll<TLeft, TRight, TResult>(TLeft left, IEnumerable<TRight> right, Action<long, TLeft, TRight, TResult> action)
        {
            long i = 0;
            foreach (var r in right)
                action(i++, left, r, expressionCache.Get<TLeft, TRight, TResult>()(left, r));
        }


        public BinaryInvoker(Func<Expression, Expression, BinaryExpression> binaryExpressionFactory)
        {
            expressionCache = new BinaryExpressionCache(binaryExpressionFactory);
        }
    }
}
