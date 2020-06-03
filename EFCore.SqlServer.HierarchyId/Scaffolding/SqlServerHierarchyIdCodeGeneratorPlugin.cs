using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;

namespace Microsoft.EntityFrameworkCore.SqlServer.Scaffolding
{
    /// <summary>
    /// HierarchyId code generator plugin <see cref="ProviderCodeGeneratorPlugin"/>.
    /// </summary>
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
