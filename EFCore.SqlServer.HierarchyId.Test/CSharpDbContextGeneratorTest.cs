using System;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.SqlServer.Internal;
using Xunit;

namespace Microsoft.EntityFrameworkCore.SqlServer;

public class CSharpDbContextGeneratorTest : ModelCodeGeneratorTestBase
{
    [ConditionalFact]
    public void Generates_context_with_UseHierarchyId()
        => Test(
            modelBuilder =>
            {
                modelBuilder.Entity(
                    "Patriarch",
                    b =>
                    {
                        b.Property<HierarchyId>("Id");
                        b.HasKey("Id");
                        b.Property<string>("Name");
                    });
            },
            new ModelCodeGenerationOptions {UseDataAnnotations = false},
            code =>
            {
                AssertFileContents(
                    @"using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TestNamespace
{
    public partial class TestDbContext : DbContext
    {
        public TestDbContext()
        {
        }

        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Patriarch> Patriarch { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer(""Initial Catalog=TestDatabase"", x => x.UseHierarchyId());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
",
                    code.ContextFile);
            });
}
