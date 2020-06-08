using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using Xunit;
using Xunit.Operators;

namespace Microsoft.EntityFrameworkCore
{
    public class HierarchyIdTests: IDisposable
    {
        private OperatorAsserters asserter;
        private IBooleanOperatorAsserter[] asserters;
        public HierarchyIdTests()
        {
            asserter = new OperatorAsserters();
            asserters = new[]
            {
                asserter.Equal,
                asserter.NotEqual,
                asserter.LessThan,
                asserter.LessThanOrEqual,
                asserter.GreaterThan,
                asserter.GreaterThanOrEqual
            };
        }

        [Fact]
        public void Null_hierarchyId_against_null()
        {
            HierarchyId hid = null;
            foreach (var asserter in asserters)
            {
                if (asserter == this.asserter.Equal)
                {
                    asserter.True<HierarchyId, HierarchyId>(hid, null);
                    asserter.True<HierarchyId, HierarchyId>(null, hid);

                }
                else
                {
                    asserter.False<HierarchyId, HierarchyId>(hid, null);
                    asserter.False<HierarchyId, HierarchyId>(null, hid);

                }
            }
        }


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
        public void Null_hierarchyId_not_equal_to_root_hierarchy_id()
        {
            HierarchyId hid = null;
            var rootHid = HierarchyId.GetRoot();
            Assert.False(hid == rootHid);
            Assert.True(hid != rootHid);
        }

        [Fact]
        public void Root_hierarchyId_not_equal_to_null_hierarchy_id()
        {
            HierarchyId hid = null;
            var rootHid = HierarchyId.GetRoot();
            Assert.False(rootHid == hid);
            Assert.True(rootHid != hid);
        }

        [Fact]
        public void Null_not_equal_to_root_hierarchy_id()
        {
            var rootHid = HierarchyId.GetRoot();
            Assert.False(null == rootHid);
            Assert.True(null != rootHid);
        }

        [Fact]
        public void Root_hierarchyId_not_equal_to_null()
        {
            var rootHid = HierarchyId.GetRoot();
            Assert.False(rootHid == null);
            Assert.True(rootHid != null);
        }

        [Fact]
        public void Null_hierarchyid_equal_to_null_hierarchyid()
        {
            HierarchyId hid1 = null;
            HierarchyId hid2 = null;
            Assert.True(hid1 == hid2);
            Assert.True(hid2 == hid1);
            Assert.False(hid1 != hid2);
            Assert.False(hid2 != hid1);
        }

        [Fact]
        public void Ordered_ascending_hierarchyId_is_null()
        {
            HierarchyId nullHid = null;
            var rootHid = HierarchyId.GetRoot();
            var val = new[] { null, HierarchyId.GetRoot() }.OrderBy(x => x).ElementAt(0);
            Assert.True(val == nullHid);
            Assert.True(nullHid == val);
            Assert.True(val != rootHid);
            Assert.True(rootHid != val);
            Assert.True(val?.ToString() == null);
        }

        [Fact]
        public void Ordered_descending_hierarchyId_is_not_null()
        {
            HierarchyId nullHid = null;
            var rootHid = HierarchyId.GetRoot();
            var val = new[] { null, HierarchyId.GetRoot() }.OrderByDescending(x => x).ElementAt(0);
            Assert.True(val != nullHid);
            Assert.True(nullHid != val);
            Assert.True(val == rootHid);
            Assert.True(rootHid == val);
            Assert.True(val.ToString() == rootHid.ToString());
        }

        [Fact]
        public void Min_non_null_hierarchyId_is_not_null()
        {
            HierarchyId nullHid = null;
            var rootHid = HierarchyId.GetRoot();
            var val = new[] { null, HierarchyId.GetRoot() }.Min();
            Assert.True(val != nullHid);
            Assert.True(nullHid != val);
            Assert.True(val == rootHid);
            Assert.True(rootHid == val);
            Assert.True(val.ToString() == rootHid.ToString());
        }

        [Fact]
        public void Max_non_null_hierarchyId_is_not_null()
        {
            HierarchyId nullHid = null;
            var rootHid = HierarchyId.GetRoot();
            var val = new[] { null, HierarchyId.GetRoot() }.Max();
            Assert.True(val != nullHid);
            Assert.True(nullHid != val);
            Assert.True(val == rootHid);
            Assert.True(rootHid == val);
            Assert.True(val.ToString() == rootHid.ToString());
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
        public void Hierarchyid_operators_same_as_int_operators()
        {
            
            void performAssertions((int? intVal, HierarchyId hid) current, (int? intVal, HierarchyId hid) previous)
            {
                foreach (var asserter in asserters)
                {
                    {
                        var result = asserter.Invoke(current.intVal, previous.intVal);
                        if (result) asserter.True(current.hid, previous.hid);
                        else asserter.False(current.hid, previous.hid);
                    }

                    //test when the values are swapped
                    {
                        var result = asserter.Invoke(previous.intVal, current.intVal);
                        if (result) asserter.True(previous.hid, current.hid);
                        else asserter.False(previous.hid, current.hid);
                    }
                }
            }

            var vals = new int?[] { null, null, 0, 0, 1, 1, 2, 2 }.Select(x => (intVal: x, hid: ConvertIntToHierarchyId(x)));

            {
                var testVals = vals.OrderBy(x => x.intVal).ToArray();
                for (int ndx = 0; ndx < testVals.Length; ndx++)
                {
                    var current = testVals[ndx];
                    var previous = ndx == 0 ? current : testVals[ndx - 1];
                    performAssertions(current, previous);
                }
            }

            {
                var testVals = vals.OrderByDescending(x => x.intVal).ToArray();
                for (int ndx = 0; ndx < testVals.Length; ndx++)
                {
                    var current = testVals[ndx];
                    var previous = ndx == 0 ? current : testVals[ndx - 1];
                    performAssertions(current, previous);
                }
            }

        }

        [Fact]
        public void Hierarchyid_operators_same_as_sqlhierarchyid_operators()
        {
            void performAssertions((SqlHierarchyId sqlHid, HierarchyId hid) current, (SqlHierarchyId sqlHid, HierarchyId hid) previous)
            {
                foreach (var asserter in asserters)
                {
                    //theres a bug in dotmorten's sql server types (https://github.com/dotMorten/Microsoft.SqlServer.Types/issues/36)
                    //where the underlying comparer is using <= for >=
                    if (asserter == this.asserter.GreaterThanOrEqual)
                        continue;

                    bool getResultAsBool(SqlHierarchyId left, SqlHierarchyId right)
                    {
                        var result = asserter.Invoke<SqlHierarchyId, SqlHierarchyId, SqlBoolean>(left, right);
                        if (result.IsNull)
                        {
                            if (left.IsNull != right.IsNull && asserter == this.asserter.NotEqual)
                                return true;
                            if (left.IsNull && right.IsNull && asserter == this.asserter.Equal)
                                return true;
                        }

                        return result.IsTrue;
                    }

                    {
                        var result = getResultAsBool(current.sqlHid, previous.sqlHid);
                        if (result) asserter.True(current.hid, previous.hid);
                        else asserter.False(current.hid, previous.hid);
                    }

                    //test when the values are swapped
                    {
                        var result = getResultAsBool(previous.sqlHid, current.sqlHid);
                        if (result) asserter.True(previous.hid, current.hid);
                        else asserter.False(previous.hid, current.hid);
                    }
                }
            }

            var vals = new int?[] { null, null, 0, 0, 1, 1, 2, 2 }.Select(x => (sqlHid: ConvertIntToSqlHierarchyId(x), hid: ConvertIntToHierarchyId(x)));

            {
                var testVals = vals.OrderBy(x => x.sqlHid).ToArray();
                for (int ndx = 0; ndx < testVals.Length; ndx++)
                {
                    var current = testVals[ndx];
                    var previous = ndx == 0 ? current : testVals[ndx - 1];
                    performAssertions(current, previous);
                }
            }

            {
                var testVals = vals.OrderByDescending(x => x.sqlHid).ToArray();
                for (int ndx = 0; ndx < testVals.Length; ndx++)
                {
                    var current = testVals[ndx];
                    var previous = ndx == 0 ? current : testVals[ndx - 1];
                    performAssertions(current, previous);
                }
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


        private static HierarchyId ConvertIntToHierarchyId(int? val)
        {
            if (val == null)
                return null;
            else if (val == 0)
                return HierarchyId.GetRoot();
            else
                return HierarchyId.Parse($"/{val}/");
        }

        private static SqlHierarchyId ConvertIntToSqlHierarchyId(int? val)
        {
            if (val == null)
                return SqlHierarchyId.Null;
            else if (val == 0)
                return SqlHierarchyId.GetRoot();
            else
                return SqlHierarchyId.Parse($"/{val}/");
        }

        private int GetHierarchyIdComparisonResult(string hid1, string hid2)
            => HierarchyId.Parse(hid1).CompareTo(HierarchyId.Parse(hid2));

        private int GetSqlHierarchyIdComparisonResult(string hid1, string hid2)
            => SqlHierarchyId.Parse(hid1).CompareTo(SqlHierarchyId.Parse(hid2));

        private bool GetHierarchyIdEqualsResult(string hid1, string hid2)
            => HierarchyId.Parse(hid1).Equals(HierarchyId.Parse(hid2));

        private bool GetSqlHierarchyIdEqualsResult(string hid1, string hid2)
            => SqlHierarchyId.Parse(hid1).Equals(SqlHierarchyId.Parse(hid2));

        public void Dispose()
        {
            asserter = null;
            asserters = null;
        }
    }

}
