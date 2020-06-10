using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.SqlServer.Storage
{
    /// <summary>
    /// maps HierarchyIds to sqlserver's hierarchyid data type
    /// </summary>
    internal class SqlServerHierarchyIdTypeMappingSourcePlugin : IRelationalTypeMappingSourcePlugin
    {
        /// <summary>
        /// the type name from sql server
        /// </summary>
        public const string SqlServerTypeName = "hierarchyid";


        /// <summary>
        ///     Finds the corresponding mapping for the hierarchyid type
        /// </summary>
        /// <param name="mappingInfo"> The mapping info to use to create the mapping. </param>
        /// <returns> The type mapping, or <c>null</c> if none could be found. </returns>
        public virtual RelationalTypeMapping FindMapping(in RelationalTypeMappingInfo mappingInfo)
        {
            var clrType = mappingInfo.ClrType ?? typeof(HierarchyId);
            var storeTypeName = mappingInfo.StoreTypeName;

            return typeof(HierarchyId).IsAssignableFrom(clrType) || storeTypeName == SqlServerTypeName
                ? new SqlServerHierarchyIdTypeMapping(SqlServerTypeName, clrType)
                : null;
        }
    }
}
