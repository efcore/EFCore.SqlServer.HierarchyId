using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;
using Xunit;

namespace Microsoft.EntityFrameworkCore
{
    public class HierarchyIdTests
    {
        [Fact]
        public void Null_hierarchyId_equals_null()
        {
            HierarchyId hid = null;
            Assert.True(hid == null);
        }

        [Fact]
        public void Null_equals_null_hierarchyid()
        {
            HierarchyId hid = null;
            Assert.True(null == hid);
        }

        [Fact]
        public void Ordered_ascending_hierarchyId_is_null()
        {
            HierarchyId nullHid = null;
            var rootHid = HierarchyId.GetRoot();
            var val = new[] { nullHid, rootHid }.OrderBy(x => x).ElementAt(0);
            Assert.True(val == nullHid);
            Assert.True(nullHid == val);
            Assert.True(val != rootHid);
            Assert.True(rootHid != val);
        }

        [Fact]
        public void Ordered_descending_hierarchyId_is_not_null()
        {
            HierarchyId nullHid = null;
            var rootHid = HierarchyId.GetRoot();
            var val = new[] { nullHid, rootHid }.OrderByDescending(x => x).ElementAt(0);
            Assert.True(val != nullHid);
            Assert.True(nullHid != val);
            Assert.True(val == rootHid);
            Assert.True(rootHid == val);
        }

        [Fact]
        public void Min_non_null_hierarchyId_is_not_null()
        {
            HierarchyId nullHid = null;
            var rootHid = HierarchyId.GetRoot();
            var val = new[] { nullHid, rootHid }.Min();
            Assert.True(val != nullHid);
            Assert.True(nullHid != val);
            Assert.True(val == rootHid);
            Assert.True(rootHid == val);
        }

        [Fact]
        public void Max_non_null_hierarchyId_is_not_null()
        {
            HierarchyId nullHid = null;
            var rootHid = HierarchyId.GetRoot();
            var val = new[] { nullHid, rootHid }.Max();
            Assert.True(val != nullHid);
            Assert.True(nullHid != val);
            Assert.True(val == rootHid);
            Assert.True(rootHid == val);
        }



        [Fact]
        public void Min_non_null_hierarchyId_string_is_not_null()
        {
            string nullHid = null;
            var rootHid = HierarchyId.GetRoot().ToString();
            var val = new[] { nullHid, rootHid }.Min();
            Assert.True(val != nullHid);
            Assert.True(nullHid != val);
            Assert.True(val == rootHid);
            Assert.True(rootHid == val);
        }

        [Fact]
        public void Max_non_null_hierarchyId_string_is_not_null()
        {
            string nullHid = null;
            var rootHid = HierarchyId.GetRoot().ToString();
            var val = new[] { nullHid, rootHid }.Max();
            Assert.True(val != nullHid);
            Assert.True(nullHid != val);
            Assert.True(val == rootHid);
            Assert.True(rootHid == val);
        }

        

        [Fact]
        public void Hierarchyid_operators_same_as_sqlhierarchyid_operators()
        {
            {
                var x = (int?)null;
                var y = (int?)1;
                
                Assert.True(x != y);
                Assert.False(x == y);
                Assert.False(x <  y);
                Assert.False(x <= y);
                Assert.False(x >  y);
                Assert.False(x >= y);

                Assert.True(y != x);
                Assert.False(y == x);
                Assert.False(y <  x);
                Assert.False(y <= x);
                Assert.False(y >  x);
                Assert.False(y >= x);
            }
            {
                var x = SqlHierarchyId.Null;
                var y = SqlHierarchyId.GetRoot();

                Assert.True(!(x != y).IsFalse); //special case
                Assert.False((x == y).IsTrue);
                Assert.False((x < y).IsTrue);
                Assert.False((x <= y).IsTrue);
                Assert.False((x > y).IsTrue);
                Assert.False((x >= y).IsTrue);

                Assert.True(!(y != x).IsFalse); //special case
                Assert.False((y == x).IsTrue);
                Assert.False((y < x).IsTrue);
                Assert.False((y <= x).IsTrue);
                Assert.False((y > x).IsTrue);
                Assert.False((y >= x).IsTrue);
            }

            {
                var x = (HierarchyId)null;
                var y = HierarchyId.GetRoot();

                Assert.True(x != y);
                Assert.False(x == y);
                Assert.False(x < y);
                Assert.False(x <= y);
                Assert.False(x > y);
                Assert.False(x >= y);

                Assert.True(y != x);
                Assert.False(y == x);
                Assert.False(y < x);
                Assert.False(y <= x);
                Assert.False(y > x);
                Assert.False(y >= x);
            }


        }

        [Fact]
        public void Hierarchyid_compareto_same_as_sqlhierarchyid_compareto()
        {
            Assert.ThrowsAny<Exception>(() => SqlHierarchyId.GetRoot().CompareTo(null));
            Assert.ThrowsAny<Exception>(() => HierarchyId.GetRoot().CompareTo(null));

            var tests = new[]
            {
                new { hid1 = "/", hid2 = "/"},
                new { hid1 = "/", hid2 = "/1/"},
                new { hid1 = "/1/", hid2 = "/1/"},
                new { hid1 = "/1/", hid2 = "/2/"},
                new { hid1 = "/1/1/", hid2 = "/2/1/"},
                new { hid1 = "/1/1/", hid2 = "/2/2/"},
                new { hid1 = "/1/1/", hid2 = "/2/2/1/"},
            };

            foreach(var t in tests)
            {
                Assert.Equal(GetSqlHierarchyIdComparisonResult(t.hid1, t.hid2), GetHierarchyIdComparisonResult(t.hid1, t.hid2));
                Assert.Equal(GetSqlHierarchyIdComparisonResult(t.hid2, t.hid1), GetHierarchyIdComparisonResult(t.hid2, t.hid1));
            }

        }

        [Fact]
        public void Hierarchyid_equals_same_as_sqlhierarchyid_equals()
        {
            Assert.False(SqlHierarchyId.GetRoot().Equals(null));
            Assert.False(HierarchyId.GetRoot().Equals(null));

            var tests = new[]
            {
                new { hid1 = "/", hid2 = "/"},
                new { hid1 = "/", hid2 = "/1/"},
                new { hid1 = "/1/", hid2 = "/1/"},
                new { hid1 = "/1/", hid2 = "/2/"},
                new { hid1 = "/1/1/", hid2 = "/2/1/"},
                new { hid1 = "/1/1/", hid2 = "/2/2/"},
                new { hid1 = "/1/1/", hid2 = "/2/2/1/"},
            };

            foreach (var t in tests)
            {
                Assert.Equal(GetSqlHierarchyIdEqualsResult(t.hid1, t.hid2), GetHierarchyIdEqualsResult(t.hid1, t.hid2));
                Assert.Equal(GetSqlHierarchyIdEqualsResult(t.hid2, t.hid1), GetHierarchyIdEqualsResult(t.hid2, t.hid1));
            }

        }



        private int GetHierarchyIdComparisonResult(string hid1, string hid2)
            => HierarchyId.Parse(hid1).CompareTo(HierarchyId.Parse(hid2));

        private int GetSqlHierarchyIdComparisonResult(string hid1, string hid2)
            => SqlHierarchyId.Parse(hid1).CompareTo(SqlHierarchyId.Parse(hid2));

        private bool GetHierarchyIdEqualsResult(string hid1, string hid2)
            => HierarchyId.Parse(hid1).Equals(HierarchyId.Parse(hid2));

        private bool GetSqlHierarchyIdEqualsResult(string hid1, string hid2)
            => SqlHierarchyId.Parse(hid1).Equals(SqlHierarchyId.Parse(hid2));

    }

}
