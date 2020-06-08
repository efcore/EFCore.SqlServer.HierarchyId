using System.Linq.Expressions;

namespace Xunit.Operators
{
    /// <summary>
    /// Uses static operators for tests
    /// </summary>
    public class OperatorAsserters
    {
        /// <summary>
        /// Uses the == operator
        /// </summary>
        public readonly IBooleanOperatorAsserter Equal = new BooleanOperatorAsserter(Expression.Equal, "==");
        /// <summary>
        /// Uses the != operator
        /// </summary>
        public readonly IBooleanOperatorAsserter NotEqual = new BooleanOperatorAsserter(Expression.NotEqual, "!=");
        /// <summary>
        /// Uses the &lt; operator
        /// </summary>
        public readonly IBooleanOperatorAsserter LessThan = new BooleanOperatorAsserter(Expression.LessThan, "<");
        /// <summary>
        /// Uses the &lt;= operator
        /// </summary>
        public readonly IBooleanOperatorAsserter LessThanOrEqual = new BooleanOperatorAsserter(Expression.LessThanOrEqual, "<=");
        /// <summary>
        /// Uses the &gt; operator
        /// </summary>
        public readonly IBooleanOperatorAsserter GreaterThan = new BooleanOperatorAsserter(Expression.GreaterThan, ">");
        /// <summary>
        /// Uses the &gt;= operator
        /// </summary>
        public readonly IBooleanOperatorAsserter GreaterThanOrEqual = new BooleanOperatorAsserter(Expression.GreaterThanOrEqual, ">=");
    }
}

