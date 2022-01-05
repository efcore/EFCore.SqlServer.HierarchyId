using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.EntityFrameworkCore.SqlServer;

#pragma warning disable EF1001

public class RelationalScaffoldingModelFactoryTest
{
    private readonly IScaffoldingModelFactory _factory;
    private readonly TestOperationReporter _reporter;

    private static readonly DatabaseModel Database;
    private static readonly DatabaseTable Table;
    private static readonly DatabaseColumn IdColumn;
    private static readonly DatabasePrimaryKey IdPrimaryKey;

    static RelationalScaffoldingModelFactoryTest()
    {
        Database = new DatabaseModel();
        Table = new DatabaseTable { Database = Database, Name = "Foo" };
        IdColumn = new DatabaseColumn
        {
            Table = Table,
            Name = "Id",
            StoreType = "int"
        };
        IdPrimaryKey = new DatabasePrimaryKey
        {
            Table = Table,
            Name = "IdPrimaryKey",
            Columns = { IdColumn }
        };
    }

    public RelationalScaffoldingModelFactoryTest()
    {
        _reporter = new TestOperationReporter();

        var assembly = typeof(RelationalScaffoldingModelFactoryTest).Assembly;
        _factory = new DesignTimeServicesBuilder(assembly, assembly, _reporter, new string[0])
            .CreateServiceCollection("Microsoft.EntityFrameworkCore.SqlServer")
            .AddSingleton<IScaffoldingModelFactory, FakeScaffoldingModelFactory>()
            .BuildServiceProvider(validateScopes: true)
            .GetRequiredService<IScaffoldingModelFactory>();

        _reporter.Clear();
    }

    [ConditionalFact]
    public void Loads_HierarchyId_columns()
    {
        var info = new DatabaseModel
        {
            Tables =
            {
                new DatabaseTable
                {
                    Database = Database,
                    Name = "Jobs",
                    Columns =
                    {
                        IdColumn,
                        new DatabaseColumn
                        {
                            Table = Table,
                            Name = "occupation",
                            StoreType = "nvarchar(max)",
                            DefaultValueSql = "\"dev\""
                        },
                        new DatabaseColumn
                        {
                            Table = Table,
                            Name = "salary",
                            StoreType = "int",
                            IsNullable = true
                        },
                        new DatabaseColumn
                        {
                            Table = Table,
                            Name = "hierarchy",
                            StoreType = "HierarchyId"
                        }
                    },
                    PrimaryKey = IdPrimaryKey
                }
            }
        };

        var entityType =
            (EntityType)_factory.Create(info, new ModelReverseEngineerOptions { NoPluralize = true }).FindEntityType("Jobs");

        Assert.Collection(
            entityType.GetProperties(),
            pk =>
            {
                Assert.Equal("Id", pk.Name);
                Assert.Equal(typeof(int), pk.ClrType);
            },
            column =>
            {
                Assert.Equal("hierarchy", column.GetColumnBaseName());
                Assert.Equal(typeof(HierarchyId), column.ClrType);
            },
            column =>
            {
                Assert.Equal("occupation", column.GetColumnBaseName());
                Assert.Equal(typeof(string), column.ClrType);
            },
            column =>
            {
                Assert.Equal("salary", column.GetColumnBaseName());
                Assert.Equal(typeof(int?), column.ClrType);
            });
    }
}
