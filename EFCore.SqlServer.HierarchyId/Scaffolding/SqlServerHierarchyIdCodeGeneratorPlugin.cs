using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;

namespace Microsoft.EntityFrameworkCore.SqlServer.Scaffolding
{
    internal class SqlServerHierarchyIdCodeGeneratorPlugin : ProviderCodeGeneratorPlugin
    {
        public override MethodCallCodeFragment GenerateProviderOptions()
        {
            return new MethodCallCodeFragment(
                nameof(SqlServerHierarchyIdDbContextOptionsBuilderExtensions.UseHierarchyId));
        }
    }
}
