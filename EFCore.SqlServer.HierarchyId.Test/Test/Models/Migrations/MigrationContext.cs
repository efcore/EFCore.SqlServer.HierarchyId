using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.SqlServer.Test.Models.Migrations
{
    internal abstract class MigrationContext<T> : DbContext, IMigrationContext
        where T : class
    {
        public DbSet<T> TestModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options
                .UseSqlServer(
                    @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=HierarchyIdMigrationTests",
                    x => x.UseHierarchyId());

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Uses internal efcore apis.")]
        protected void RemoveVariableModelAnnotations(ModelBuilder modelBuilder)
        {
            var model = modelBuilder.Model;

            //the values of these could change between versions
            //so get rid of them for the tests
            model.RemoveAnnotation(CoreAnnotationNames.ProductVersion);
            model.RemoveAnnotation("Relational:MaxIdentifierLength");
            model.RemoveAnnotation("SqlServer:ValueGenerationStrategy");
        }

        protected abstract void SeedData(EntityTypeBuilder<T> builder);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            RemoveVariableModelAnnotations(modelBuilder);
            SeedData(modelBuilder.Entity<T>());
        }

        public abstract string GetExpectedMigrationCode(string migrationName, string @namespace);
        public abstract string GetExpectedSnapshotCode(string @namespace);
    }
}
