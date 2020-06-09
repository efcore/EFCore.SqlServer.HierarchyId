using Microsoft.SqlServer.Types;
using System;

namespace Microsoft.EntityFrameworkCore.Test.Operators
{
    public static class OperatorExtensions
    {
        /// <summary>
        /// returns the shorthand text of the operator
        /// </summary>
        /// <param name="operator"></param>
        /// <returns></returns>
        public static string AsString(this Operator @operator)
        {
            return @operator switch
            {
                Operator.Equal => "==",
                Operator.NotEqual => "!=",
                Operator.LessThan => "<",
                Operator.LessThanOrEqual => "<=",
                Operator.GreaterThan => ">",
                Operator.GreaterThanOrEqual => ">=",
                _ => $"{@operator}",
            };
        }

        /// <summary>
        /// returns the boolean result of the operator's operation
        /// </summary>
        /// <param name="operator">the operator to execute the operation</param>
        /// <param name="operands">the operands to execute the operation against</param>
        /// <returns></returns>
        public static bool Execute(this Operator @operator, (int? lhs, int? rhs) operands)
        {
            var (lhs, rhs) = operands;
            return @operator switch
            {
                Operator.Equal => lhs == rhs,
                Operator.NotEqual => lhs != rhs,
                Operator.LessThan => lhs < rhs,
                Operator.LessThanOrEqual => lhs <= rhs,
                Operator.GreaterThan => lhs > rhs,
                Operator.GreaterThanOrEqual => lhs >= rhs,
                _ => throw new InvalidOperationException($"The operator {@operator.AsString()} is invalid for int?"),
            };
        }

        /// <summary>
        /// returns the boolean result of the operator's operation
        /// </summary>
        /// <param name="operator">the operator to execute the operation</param>
        /// <param name="operands">the operands to execute the operation against</param>
        /// <returns></returns>
        public static bool Execute(this Operator @operator, (HierarchyId lhs, HierarchyId rhs) operands)
        {
            var (lhs, rhs) = operands;
            return @operator switch
            {
                Operator.Equal => lhs == rhs,
                Operator.NotEqual => lhs != rhs,
                Operator.LessThan => lhs < rhs,
                Operator.LessThanOrEqual => lhs <= rhs,
                Operator.GreaterThan => lhs > rhs,
                Operator.GreaterThanOrEqual => lhs >= rhs,
                _ => throw new InvalidOperationException($"The operator {@operator.AsString()} is invalid for {nameof(HierarchyId)}"),
            };
        }

        /// <summary>
        /// returns the boolean result of the operator's operation
        /// </summary>
        /// <param name="operator">the operator to execute the operation</param>
        /// <param name="operands">the operands to execute the operation against</param>
        /// <returns></returns>
        public static bool Execute(this Operator @operator, (SqlHierarchyId lhs, SqlHierarchyId rhs) operands)
        {
            var (lhs, rhs) = operands;
            return @operator switch
            {
                Operator.Equal => (lhs.IsNull && rhs.IsNull) || (lhs == rhs).IsTrue,
                Operator.NotEqual => (lhs.IsNull != rhs.IsNull) || (lhs != rhs).IsTrue,
                Operator.LessThan => (lhs < rhs).IsTrue,
                Operator.LessThanOrEqual => (lhs <= rhs).IsTrue,
                Operator.GreaterThan => (lhs > rhs).IsTrue,
                Operator.GreaterThanOrEqual => (lhs >= rhs).IsTrue,
                _ => throw new InvalidOperationException($"The operator {@operator.AsString()} is invalid for {nameof(SqlHierarchyId)}"),
            };
        }

        /// <summary>
        /// returns a detailed failure message of the assertion
        /// </summary>
        /// <typeparam name="TOperand"></typeparam>
        /// <param name="operator"></param>
        /// <param name="operands"></param>
        /// <returns></returns>
        public static string GetFailureMessage<TOperand>(this Operator @operator, (TOperand lhs, TOperand rhs) operands)
        {
            var typeName = typeof(TOperand).Name.QuoteWith("'");
            var opAsString = @operator.AsString();
            return $"Operator {typeName} {opAsString} {typeName} " +
               $"failed for {operands.lhs.Stringify()} {opAsString} {operands.rhs.Stringify()}";
        }

    }
}
