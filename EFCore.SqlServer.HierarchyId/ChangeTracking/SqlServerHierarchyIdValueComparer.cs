using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.EntityFrameworkCore.ChangeTracking
{
    /// <summary>
    ///     <para>
    ///         Specifies custom value snapshotting and comparison for
    ///         CLR types that cannot be compared with <see cref="object.Equals(object, object)" />
    ///         and/or need a deep copy when taking a snapshot. For example, arrays of primitive types
    ///         will require both if mutation is to be detected.
    ///     </para>
    ///     <para>
    ///         Snapshotting is the process of creating a copy of the value into a snapshot so it can
    ///         later be compared to determine if it has changed. For some types, such as collections,
    ///         this needs to be a deep copy of the collection rather than just a shallow copy of the
    ///         reference.
    ///     </para>
    /// </summary>
    public class SqlServerHierarchyIdValueComparer : ValueComparer<HierarchyId>
    {
        /// <summary>
        ///     <para>
        ///         Creates a new <see cref="SqlServerHierarchyIdValueComparer" /> with the given comparison and
        ///         snapshotting expressions.
        ///     </para>
        ///     <para>
        ///         Snapshotting is the process of creating a copy of the value into a snapshot so it can
        ///         later be compared to determine if it has changed. For some types, such as collections,
        ///         this needs to be a deep copy of the collection rather than just a shallow copy of the
        ///         reference.
        ///     </para>
        /// </summary>
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
