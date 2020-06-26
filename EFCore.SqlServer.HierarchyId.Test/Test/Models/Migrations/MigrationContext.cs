using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Metadata.Internal;
using System;

namespace Microsoft.EntityFrameworkCore.SqlServer.Test.Models.Migrations
{
    internal abstract class MigrationContext<T> : DbContext
        where T : class
    {
        protected Type ModelType { get; } = typeof(T);

        private Type _thisType;
        protected Type ThisType => _thisType ??= GetType();

        public DbSet<T> TestModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options
                .UseSqlServer(
                    @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=HierarchyIdMigrationTests",
                    x => x.UseHierarchyId());

        /// <summary>
        /// Removes annotations from the model that can
        /// change between versions of ef.
        /// This should be called during OnModelCreating
        /// </summary>
        /// <param name="modelBuilder"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Uses internal efcore apis.")]
        protected void RemoveVariableModelAnnotations(ModelBuilder modelBuilder)
        {
            var model = modelBuilder.Model;

            //the values of these could change between versions
            //so get rid of them for the tests
            model.RemoveAnnotation(CoreAnnotationNames.ProductVersion);
            model.RemoveAnnotation(RelationalAnnotationNames.MaxIdentifierLength);
            model.RemoveAnnotation(SqlServerAnnotationNames.ValueGenerationStrategy);
        }

        public abstract string GetExpectedMigrationCode(string migrationName, string rootNamespace);
        public abstract string GetExpectedSnapshotCode(string rootNamespace);
    }
}
