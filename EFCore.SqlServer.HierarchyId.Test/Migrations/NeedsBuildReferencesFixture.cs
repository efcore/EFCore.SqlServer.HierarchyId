using Microsoft.EntityFrameworkCore.TestUtilities;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.EntityFrameworkCore.SqlServer.Migrations
{
    /// <summary>
    /// use this class fixture when you need to get build references
    /// and are compiling against .net core & dotnetframework
    /// </summary>
    public class NeedsBuildReferencesFixture
    {
        public NeedsBuildReferencesFixture()
        {
            AssemblyUtils.SetEntryAssembly();
        }

        private IEnumerable<BuildReference> _getRequiredReferences()
        {
            if (AssemblyUtils.WasEntryAssemblyUpdated)
            {
                return new[] {
                    BuildReference.ByName("mscorlib"), };
            }

            return new BuildReference[0];
        }

        /// <summary>
        /// returns a new collection of required build references
        /// if the entry assembly was modified, mscorlib will be returned
        /// as a buildreference
        /// </summary>
        public ICollection<BuildReference> GetBuildReferences(params BuildReference[] buildReferences)
            => new List<BuildReference>(_getRequiredReferences().Concat(buildReferences));


    }
    
}
