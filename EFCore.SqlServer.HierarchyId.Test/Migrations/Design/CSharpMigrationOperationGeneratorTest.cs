// Adapted From: https://github.com/dotnet/efcore/blob/v3.1.0/test/EFCore.Design.Tests/Migrations/Design/CSharpMigrationOperationGeneratorTest.cs
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.SqlServer.Storage;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Microsoft.EntityFrameworkCore.SqlServer.Migrations.Design
{
    public class CSharpMigrationOperationGeneratorTest : IClassFixture<NeedsBuildReferencesFixture>
    {
        private readonly NeedsBuildReferencesFixture _fixture;

        private static readonly string _eol = Environment.NewLine;

        private static readonly HierarchyId _rootHid = HierarchyId.GetRoot();
        private static readonly HierarchyId _childHid1 = HierarchyId.Parse("/1/");
        private static readonly HierarchyId _childHid2 = HierarchyId.Parse("/2/");

        public CSharpMigrationOperationGeneratorTest(NeedsBuildReferencesFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void InsertDataOperation_all_args()
        {
            Test(
                new InsertDataOperation
                {
                    Schema = "dbo",
                    Table = "People",
                    Columns = new[] { "Id", "Full Name", "Hierarchy" },
                    Values = new object[,]
                    {
                        { 0, null, null },
                        { 1, "Eddard Stark", _rootHid },
                        { 2, "Robb Stark", _childHid1 },
                    }
                },
                "mb.InsertData("
                + _eol
                + "    schema: \"dbo\","
                + _eol
                + "    table: \"People\","
                + _eol
                + "    columns: new[] { \"Id\", \"Full Name\", \"Hierarchy\" },"
                + _eol
                + "    values: new object[,]"
                + _eol
                + "    {"
                + _eol
                + "        { 0, null, null },"
                + _eol
                + "        { 1, \"Eddard Stark\", Microsoft.EntityFrameworkCore.HierarchyId.Parse(\"/\") },"
                + _eol
                + "        { 2, \"Robb Stark\", Microsoft.EntityFrameworkCore.HierarchyId.Parse(\"/1/\") }"
                + _eol
                + "    });",
                o =>
                {
                    Assert.Equal("dbo", o.Schema);
                    Assert.Equal("People", o.Table);
                    Assert.Equal(3, o.Columns.Length);
                    Assert.Equal(3, o.Values.GetLength(0));
                    Assert.Equal(3, o.Values.GetLength(1));
                    Assert.Equal("Robb Stark", o.Values[2, 1]);
                    Assert.Equal(_rootHid, o.Values[1, 2]);
                    Assert.Equal(_childHid1, o.Values[2, 2]);
                });
        }

        [Fact]
        public void InsertDataOperation_required_args()
        {
            Test(
                new InsertDataOperation
                {
                    Table = "People",
                    Columns = new[] { "Hierarchy" },
                    Values = new object[,] { { _rootHid } }
                },
                "mb.InsertData("
                + _eol
                + "    table: \"People\","
                + _eol
                + "    column: \"Hierarchy\","
                + _eol
                + "    value: Microsoft.EntityFrameworkCore.HierarchyId.Parse(\"/\"));",
                o =>
                {
                    Assert.Equal("People", o.Table);
                    Assert.Single(o.Columns);
                    Assert.Equal(1, o.Values.GetLength(0));
                    Assert.Equal(1, o.Values.GetLength(1));
                    Assert.Equal(_rootHid, o.Values[0, 0]);
                });
        }

        [Fact]
        public void InsertDataOperation_required_args_composite()
        {
            Test(
                new InsertDataOperation
                {
                    Table = "People",
                    Columns = new[] { "First Name", "Last Name", "Hierarchy" },
                    Values = new object[,] { { "John", "Snow", _childHid2 } }
                },
                "mb.InsertData("
                + _eol
                + "    table: \"People\","
                + _eol
                + "    columns: new[] { \"First Name\", \"Last Name\", \"Hierarchy\" },"
                + _eol
                + "    values: new object[] { \"John\", \"Snow\", Microsoft.EntityFrameworkCore.HierarchyId.Parse(\"/2/\") });",
                o =>
                {
                    Assert.Equal("People", o.Table);
                    Assert.Equal(3, o.Columns.Length);
                    Assert.Equal(1, o.Values.GetLength(0));
                    Assert.Equal(3, o.Values.GetLength(1));
                    Assert.Equal("Snow", o.Values[0, 1]);
                    Assert.Equal(_childHid2, o.Values[0, 2]);
                });
        }

        [Fact]
        public void InsertDataOperation_required_args_multiple_rows()
        {
            Test(
                new InsertDataOperation
                {
                    Table = "People",
                    Columns = new[] { "Hierarchy" },
                    Values = new object[,] { { _childHid1 }, { _childHid2 } }
                },
                "mb.InsertData("
                + _eol
                + "    table: \"People\","
                + _eol
                + "    column: \"Hierarchy\","
                + _eol
                + "    values: new object[]"
                + _eol
                + "    {"
                + _eol
                + "        Microsoft.EntityFrameworkCore.HierarchyId.Parse(\"/1/\"),"
                + _eol
                + "        Microsoft.EntityFrameworkCore.HierarchyId.Parse(\"/2/\")"
                + _eol
                + "    });",
                o =>
                {
                    Assert.Equal("People", o.Table);
                    Assert.Single(o.Columns);
                    Assert.Equal(2, o.Values.GetLength(0));
                    Assert.Equal(1, o.Values.GetLength(1));
                    Assert.Equal(_childHid1, o.Values[0, 0]);
                    Assert.Equal(_childHid2, o.Values[1, 0]);
                });
        }

        protected ICollection<BuildReference> GetReferences()
            => _fixture.GetBuildReferences(
                BuildReference.ByName("Microsoft.EntityFrameworkCore.Relational"),
                BuildReference.ByName("EntityFrameworkCore.SqlServer.HierarchyId.Abstractions"));


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Uses efcore internal apis")]
        private void Test<T>(T operation, string expectedCode, Action<T> assert)
            where T : MigrationOperation
        {
            var generator = new CSharpMigrationOperationGenerator(
                new CSharpMigrationOperationGeneratorDependencies(
                    new CSharpHelper(
                        new SqlServerTypeMappingSource(
                            TestServiceFactory.Instance.Create<TypeMappingSourceDependencies>(),
                            new RelationalTypeMappingSourceDependencies(
                                new IRelationalTypeMappingSourcePlugin[]
                                {
                                    new SqlServerHierarchyIdTypeMappingSourcePlugin()
                                })))));

            var builder = new IndentedStringBuilder();
            generator.Generate("mb", new[] { operation }, builder);
            var code = builder.ToString();

            Assert.Equal(expectedCode, code);

            var build = new BuildSource
            {
                Sources =
                {
                    @"
                    using Microsoft.EntityFrameworkCore.Migrations;
                    using Microsoft.EntityFrameworkCore;
                    public static class OperationsFactory
                    {
                        public static void Create(MigrationBuilder mb)
                        {
                            "
                    + code
                    + @"
                        }
                    }
                "
                }
            };

            foreach (var buildReference in GetReferences())
            {
                build.References.Add(buildReference);
            }

            var assembly = build.BuildInMemory();
            var factoryType = assembly.GetType("OperationsFactory");
            var createMethod = factoryType.GetTypeInfo().GetDeclaredMethod("Create");
            var mb = new MigrationBuilder(activeProvider: null);
            createMethod.Invoke(null, new[] { mb });
            var result = mb.Operations.Cast<T>().Single();

            assert(result);
        }
    }
}