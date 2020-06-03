using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;

namespace Microsoft.EntityFrameworkCore.SqlServer.Scaffolding
{
    public class SqlServerHierarchyIdCodeGeneratorPlugin : ProviderCodeGeneratorPlugin
    {
        /// <summary>
        ///     Generates a method chain to configure additional context options.
        /// </summary>
        /// <returns> The method chain. May be null. </returns>
        public override MethodCallCodeFragment GenerateProviderOptions()
        {
            return new MethodCallCodeFragment(
                nameof(SqlServerHierarchyIdDbContextOptionsBuilderExtensions.UseHierarchyId));
        }
    }
}
