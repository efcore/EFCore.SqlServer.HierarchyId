namespace Microsoft.EntityFrameworkCore.Test.Operators
{
    public enum Operator
    {
        /// <summary>
        /// Uses the == operator
        /// </summary>
        Equal = 0,
        /// <summary>
        /// Uses the != operator
        /// </summary>
        NotEqual = 1,
        /// <summary>
        /// Uses the &lt; operator
        /// </summary>
        LessThan = 2,
        /// <summary>
        /// Uses the &lt;= operator
        /// </summary>
        LessThanOrEqual = 3,
        /// <summary>
        /// Uses the &gt; operator
        /// </summary>
        GreaterThan = 4,
        /// <summary>
        /// Uses the &gt;= operator
        /// </summary>
        GreaterThanOrEqual = 5
    }
}
