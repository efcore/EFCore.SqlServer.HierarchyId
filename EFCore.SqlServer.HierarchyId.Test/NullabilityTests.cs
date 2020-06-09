using Microsoft.EntityFrameworkCore.Test.Operators;
using System.Linq;
using Xunit;

namespace Microsoft.EntityFrameworkCore.SqlServer
{
    public class NullabilityTests
    {
        [Fact]
        public void Null_equalTo_null_isTrue()
        {
            const Operator @operator = Operator.Equal;

            var vals = ((HierarchyId)null, (HierarchyId)null);
            Assert.True(@operator.Execute(vals), @operator.GetFailureMessage(vals));

            var swapped = vals.Swap();
            Assert.True(@operator.Execute(swapped), @operator.GetFailureMessage(swapped));
        }

        [Fact]
        public void Null_notEqualTo_null_isTrue()
        {
            const Operator @operator = Operator.NotEqual;

            var vals = ((HierarchyId)null, (HierarchyId)null);
            Assert.False(@operator.Execute(vals), @operator.GetFailureMessage(vals));

            var swapped = vals.Swap();
            Assert.False(@operator.Execute(swapped), @operator.GetFailureMessage(swapped));
        }

        [Fact]
        public void Null_greaterThan_null_isFalse()
        {
            const Operator @operator = Operator.GreaterThan;

            var vals = ((HierarchyId)null, (HierarchyId)null);
            Assert.False(@operator.Execute(vals), @operator.GetFailureMessage(vals));

            var swapped = vals.Swap();
            Assert.False(@operator.Execute(swapped), @operator.GetFailureMessage(swapped));
        }

        [Fact]
        public void Null_greaterThanOrEqualTo_null_isFalse()
        {
            const Operator @operator = Operator.GreaterThanOrEqual;

            var vals = ((HierarchyId)null, (HierarchyId)null);
            Assert.False(@operator.Execute(vals), @operator.GetFailureMessage(vals));

            var swapped = vals.Swap();
            Assert.False(@operator.Execute(swapped), @operator.GetFailureMessage(swapped));
        }

        [Fact]
        public void Null_lessThan_null_isFalse()
        {
            const Operator @operator = Operator.LessThan;

            var vals = ((HierarchyId)null, (HierarchyId)null);
            Assert.False(@operator.Execute(vals), @operator.GetFailureMessage(vals));

            var swapped = vals.Swap();
            Assert.False(@operator.Execute(swapped), @operator.GetFailureMessage(swapped));
        }

        [Fact]
        public void Null_lessThanOrEqualTo_null_isFalse()
        {
            const Operator @operator = Operator.LessThanOrEqual;

            var vals = ((HierarchyId)null, (HierarchyId)null);
            Assert.False(@operator.Execute(vals), @operator.GetFailureMessage(vals));

            var swapped = vals.Swap();
            Assert.False(@operator.Execute(swapped), @operator.GetFailureMessage(swapped));
        }


        [Fact]
        public void Null_greaterThan_nonNull_isFalse()
        {
            const Operator @operator = Operator.GreaterThan;

            var vals = ((HierarchyId)null, HierarchyId.GetRoot());
            Assert.False(@operator.Execute(vals), @operator.GetFailureMessage(vals));
        }

        [Fact]
        public void Null_greaterThanOrEqualTo_nonNull_isFalse()
        {
            const Operator @operator = Operator.GreaterThanOrEqual;

            var vals = ((HierarchyId)null, HierarchyId.GetRoot());
            Assert.False(@operator.Execute(vals), @operator.GetFailureMessage(vals));
        }

        [Fact]
        public void Null_lessThan_nonNull_isFalse()
        {
            const Operator @operator = Operator.LessThan;

            var vals = ((HierarchyId)null, HierarchyId.GetRoot());
            Assert.False(@operator.Execute(vals), @operator.GetFailureMessage(vals));
        }

        [Fact]
        public void Null_lessThanOrEqualTo_nonNull_isFalse()
        {
            const Operator @operator = Operator.LessThanOrEqual;

            var vals = ((HierarchyId)null, HierarchyId.GetRoot());
            Assert.False(@operator.Execute(vals), @operator.GetFailureMessage(vals));
        }

        [Fact]
        public void NonNull_greaterThan_null_isFalse()
        {
            const Operator @operator = Operator.GreaterThan;

            var vals = (HierarchyId.GetRoot(), (HierarchyId)null);
            Assert.False(@operator.Execute(vals), @operator.GetFailureMessage(vals));
        }

        [Fact]
        public void NonNull_greaterThanOrEqualTo_null_isFalse()
        {
            const Operator @operator = Operator.GreaterThanOrEqual;

            var vals = (HierarchyId.GetRoot(), (HierarchyId)null);
            Assert.False(@operator.Execute(vals), @operator.GetFailureMessage(vals));
        }

        [Fact]
        public void NonNull_lessThan_null_isFalse()
        {
            const Operator @operator = Operator.LessThan;

            var vals = (HierarchyId.GetRoot(), (HierarchyId)null);
            Assert.False(@operator.Execute(vals), @operator.GetFailureMessage(vals));
        }

        [Fact]
        public void NonNull_lessThanOrEqualTo_null_isFalse()
        {
            const Operator @operator = Operator.LessThanOrEqual;

            var vals = (HierarchyId.GetRoot(), (HierarchyId)null);
            Assert.False(@operator.Execute(vals), @operator.GetFailureMessage(vals));
        }

        [Fact]
        public void NonNull_notEqualTo_null_isTrue()
        {
            const Operator @operator = Operator.NotEqual;

            var vals = (HierarchyId.GetRoot(), (HierarchyId)null);
            Assert.True(@operator.Execute(vals), @operator.GetFailureMessage(vals));
        }

        [Fact]
        public void Null_notEqualTo_nonNull_isTrue()
        {
            const Operator @operator = Operator.NotEqual;

            var vals = ((HierarchyId)null, HierarchyId.GetRoot());
            Assert.True(@operator.Execute(vals), @operator.GetFailureMessage(vals));
        }


        [Fact]
        public void Min_includingNulls_equalTo_nonNull_isTrue()
        {
            const Operator @operator = Operator.Equal;

            var min = new[] { (HierarchyId)null, HierarchyId.GetRoot() }.Min();

            var vals = (HierarchyId.GetRoot(), min);
            Assert.True(@operator.Execute(vals), @operator.GetFailureMessage(vals));

            var swapped = vals.Swap();
            Assert.True(@operator.Execute(swapped), @operator.GetFailureMessage(swapped));
        }

        [Fact]
        public void Min_includingNulls_notEqualTo_nonNull_isFalse()
        {
            const Operator @operator = Operator.NotEqual;

            var min = new[] { (HierarchyId)null, HierarchyId.GetRoot() }.Min();

            var vals = (HierarchyId.GetRoot(), min);
            Assert.False(@operator.Execute(vals), @operator.GetFailureMessage(vals));

            var swapped = vals.Swap();
            Assert.False(@operator.Execute(swapped), @operator.GetFailureMessage(swapped));
        }


        [Fact]
        public void Max_includingNulls_notEqualTo_null_isTrue()
        {
            const Operator @operator = Operator.NotEqual;

            var max = new[] { (HierarchyId)null, HierarchyId.GetRoot() }.Max();

            var vals = ((HierarchyId)null, max);
            Assert.True(@operator.Execute(vals), @operator.GetFailureMessage(vals));

            var swapped = vals.Swap();
            Assert.True(@operator.Execute(swapped), @operator.GetFailureMessage(swapped));
        }

        [Fact]
        public void Max_includingNulls_equalTo_null_isFalse()
        {
            const Operator @operator = Operator.Equal;

            var max = new[] { (HierarchyId)null, HierarchyId.GetRoot() }.Max();

            var vals = ((HierarchyId)null, max);
            Assert.False(@operator.Execute(vals), @operator.GetFailureMessage(vals));

            var swapped = vals.Swap();
            Assert.False(@operator.Execute(swapped), @operator.GetFailureMessage(swapped));
        }


        [Fact]
        public void Min_nullsOnly_equalTo_null_isTrue()
        {
            const Operator @operator = Operator.Equal;

            var min = new[] { (HierarchyId)null }.Min();

            var vals = ((HierarchyId)null, min);
            Assert.True(@operator.Execute(vals), @operator.GetFailureMessage(vals));

            var swapped = vals.Swap();
            Assert.True(@operator.Execute(swapped), @operator.GetFailureMessage(swapped));
        }

        [Fact]
        public void Min_nullsOnly_notEqualTo_null_isFalse()
        {
            const Operator @operator = Operator.NotEqual;

            var min = new[] { (HierarchyId)null }.Min();

            var vals = ((HierarchyId)null, min);
            Assert.False(@operator.Execute(vals), @operator.GetFailureMessage(vals));

            var swapped = vals.Swap();
            Assert.False(@operator.Execute(swapped), @operator.GetFailureMessage(swapped));
        }

        [Fact]
        public void Max_nullsOnly_equalTo_null_isTrue()
        {
            const Operator @operator = Operator.Equal;

            var max = new[] { (HierarchyId)null }.Max();

            var vals = ((HierarchyId)null, max);
            Assert.True(@operator.Execute(vals), @operator.GetFailureMessage(vals));

            var swapped = vals.Swap();
            Assert.True(@operator.Execute(swapped), @operator.GetFailureMessage(swapped));
        }

        [Fact]
        public void Max_nullsOnly_notEqualTo_null_isFalse()
        {
            const Operator @operator = Operator.NotEqual;

            var min = new[] { (HierarchyId)null }.Max();

            var vals = ((HierarchyId)null, min);
            Assert.False(@operator.Execute(vals), @operator.GetFailureMessage(vals));

            var swapped = vals.Swap();
            Assert.False(@operator.Execute(swapped), @operator.GetFailureMessage(swapped));
        }
    }
}
