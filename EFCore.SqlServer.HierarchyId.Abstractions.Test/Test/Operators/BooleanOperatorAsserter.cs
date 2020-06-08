using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Xunit.Operators
{
    internal class BooleanOperatorAsserter : BinaryInvoker, IBooleanOperatorAsserter
    {
        public string Operator { get; }

        private string CreateFailureMessage<TLeft, TRight>(TLeft left, TRight right)
            => $"Operator {typeof(TLeft).Name.QuoteWith("'")} {Operator} {typeof(TRight).Name.QuoteWith("'")} " +
               $"failed for {left.Stringify()} {Operator} {right.Stringify()}";

        public bool Invoke<TLeft, TRight>(TLeft left, TRight right)
            => Invoke<TLeft, TRight, bool>(left, right);

        public void InvokeAll<TLeft, TRight>(TLeft left, IEnumerable<TRight> right, Action<long, TLeft, TRight, bool> action)
            => InvokeAll<TLeft, TRight, bool>(left, right, action);

        private void AssertOne<TLeft, TRight>(TLeft left, TRight right, Action<bool, string> action)
            => action(Invoke(left, right), CreateFailureMessage(left, right));

        public void True<TLeft, TRight>(TLeft left, TRight right)
            => AssertOne(left, right, Assert.True);

        public void False<TLeft, TRight>(TLeft left, TRight right)
            => AssertOne(left, right, Assert.False);

        private void AssertAll<TLeft, TRight>(TLeft left, IEnumerable<TRight> right, Action<bool, string> action)
            => InvokeAll(left, right, (ndx, lhs, rhs, result) => action(result, $"{CreateFailureMessage(lhs, rhs)} @ {nameof(right)}[{ndx}]"));

        public void TrueAll<TLeft, TRight>(TLeft left, IEnumerable<TRight> right)
            => AssertAll(left, right, Assert.True);

        public void FalseAll<TLeft, TRight>(TLeft left, IEnumerable<TRight> right)
            => AssertAll(left, right, Assert.False);

        public BooleanOperatorAsserter(Func<Expression, Expression, BinaryExpression> opExpression, string oporator): base(opExpression)
        {
            Operator = oporator;
        }
    }

    internal static class StringExtensions
    {
        public static string QuoteWith(this string s, string value)
            => $"{value}{s}{value}";

        public static string Stringify(this object o)
            => o is null ? "null" : o.ToString();
    }
}
