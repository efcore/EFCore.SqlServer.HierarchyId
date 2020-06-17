using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.SqlServer.Test.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Microsoft.EntityFrameworkCore.SqlServer
{
    public class MigrationTests
    {
        [Fact]
        public void Migration_and_snapshot_generate_with_typed_array()
        {
            using var db = new MigrationContext1();
            ValidateSnapshotAndMigrationCode(db);
        }


        [Fact]
        public void Migration_and_snapshot_generate_with_anonymous_array()
        {
            using var db = new MigrationContext2();
            ValidateSnapshotAndMigrationCode(db);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Uses internal efcore apis")]
        private void ValidateSnapshotAndMigrationCode(MigrationContext context)
        {
            const string migrationName = "MyMigration";
            const string @namespace = "MyApp.Data";

            var expectedMigration = context.GetExpectedMigrationCode(migrationName, @namespace);
            var expectedSnapshot = context.GetExpectedSnapshotCode(@namespace);

            var reporter = new OperationReporter(
                new OperationReportHandler(
                    m => Console.WriteLine("  error: " + m),
                    m => Console.WriteLine("   warn: " + m),
                    m => Console.WriteLine("   info: " + m),
                    m => Console.WriteLine("verbose: " + m)));

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            var servicesBuilder = new DesignTimeServicesBuilder(
                assembly: assembly,
                startupAssembly: assembly,
                reporter: reporter,
                args: Array.Empty<string>()
            );
            
            var services = servicesBuilder.Build(context);
            var scaffolder = services.GetRequiredService<IMigrationsScaffolder>();

            var migration = scaffolder.ScaffoldMigration(
                migrationName,
                @namespace);

            Assert.Equal(expectedMigration, migration.MigrationCode);
            Assert.Equal(expectedSnapshot, migration.SnapshotCode);
        }
    }
}
