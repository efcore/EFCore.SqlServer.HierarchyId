using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.EntityFrameworkCore.ChangeTracking
{
    public class SqlServerHierarchyIdValueComparer : ValueComparer<HierarchyId>
    {
        /// <summary>
        ///     <para>
        ///         Creates a new <see cref="ValueComparer{T}" /> with the given comparison and
        ///         snapshotting expressions.
        ///     </para>
        ///     <para>
        ///         Snapshotting is the process of creating a copy of the value into a snapshot so it can
        ///         later be compared to determine if it has changed. For some types, such as collections,
        ///         this needs to be a deep copy of the collection rather than just a shallow copy of the
        ///         reference.
        ///     </para>
        /// </summary>
        /// <param name="equalsExpression"> The comparison expression. </param>
        /// <param name="hashCodeExpression"> The associated hash code generator. </param>
        /// <param name="snapshotExpression"> The snapshot expression. </param>
        public SqlServerHierarchyIdValueComparer()
            : base(
                GetEqualsExpression(),
                GetHashCodeExpression(),
                GetSnapshotExpression())
        {
        }

        private static Expression<Func<HierarchyId, HierarchyId, bool>> GetEqualsExpression()
        {
            var left = Expression.Parameter(typeof(HierarchyId), "left");
            var right = Expression.Parameter(typeof(HierarchyId), "right");

            return Expression.Lambda<Func<HierarchyId, HierarchyId, bool>>(
                Expression.Call(
                    left,
                    typeof(HierarchyId).GetRuntimeMethod("Equals", new[] { typeof(HierarchyId) }),
                    right),
                left,
                right);
        }

        private static Expression<Func<HierarchyId, int>> GetHashCodeExpression()
        {
            var instance = Expression.Parameter(typeof(HierarchyId), "instance");

            var ret = Expression.Lambda<Func<HierarchyId, int>>(
                Expression.Call(
                    instance,
                    typeof(HierarchyId).GetRuntimeMethod("GetHashCode", Type.EmptyTypes)),
                instance);

            return ret;
        }

        private static Expression<Func<HierarchyId, HierarchyId>> GetSnapshotExpression()
        {
            var instance = Expression.Parameter(typeof(HierarchyId), "instance");

            Expression body = Expression.Call(
                typeof(HierarchyId).GetRuntimeMethod("Parse", new[] { typeof(string) }),
                Expression.Call(
                    instance,
                    typeof(HierarchyId).GetRuntimeMethod("ToString", Type.EmptyTypes))
            );

            if (typeof(HierarchyId).FullName != "Microsoft.EntityFrameworkCore.HierarchyId")
            {
                body = Expression.Convert(body, typeof(HierarchyId));
            }

            return Expression.Lambda<Func<HierarchyId, HierarchyId>>(body, instance);
        }
    }
}
