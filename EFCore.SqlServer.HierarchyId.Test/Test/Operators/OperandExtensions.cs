namespace Microsoft.EntityFrameworkCore.Test.Operators
{
    public static class OperandExtensions
    {
        public static (TOperand lhs, TOperand rhs) Swap<TOperand>(this (TOperand lhs, TOperand rhs) operands)
            => (operands.rhs, operands.lhs);

    }
}
