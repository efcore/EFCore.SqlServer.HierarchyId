namespace Microsoft.EntityFrameworkCore.Test.Operators
{
    public static class OperandExtensions
    {
        /// <summary>
        /// returns a new value tuple with the left-hand side and right-hand side values swapped
        /// </summary>
        /// <typeparam name="TOperand"></typeparam>
        /// <param name="operands"></param>
        /// <returns></returns>
        public static (TOperand lhs, TOperand rhs) Swap<TOperand>(this (TOperand lhs, TOperand rhs) operands)
            => (operands.rhs, operands.lhs);

    }
}
