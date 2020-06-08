using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Xunit.Operators
{
    internal class BinaryExpressionCache
    {
        private readonly Func<Expression, Expression, BinaryExpression> expressionFactory;
        private readonly ConcurrentDictionary<(Type LeftType, Type RightType), object> expressions = new ConcurrentDictionary<(Type LeftType, Type RightType), object>();

        private Func<TLeft, TRight, TResult> CreateValue<TLeft, TRight, TResult>((Type LeftType, Type RightType) key)
        {
            var left = Expression.Parameter(key.LeftType, "left");
            var right = Expression.Parameter(key.RightType, "right");

            return Expression.Lambda<Func<TLeft, TRight, TResult>>(
                expressionFactory(left, right),
                left,
                right
            ).Compile();
        }

        public Func<TLeft, TRight, TResult> Get<TLeft, TRight, TResult>()
            => (Func<TLeft, TRight, TResult>)expressions.GetOrAdd((typeof(TLeft), typeof(TRight)), CreateValue<TLeft, TRight, TResult>);

        public BinaryExpressionCache(Func<Expression, Expression, BinaryExpression> binaryExpressionFactory)
        {
            this.expressionFactory = binaryExpressionFactory ?? throw new ArgumentNullException(nameof(binaryExpressionFactory));
        }

    }
}
