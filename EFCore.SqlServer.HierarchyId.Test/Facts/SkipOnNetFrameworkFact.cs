using System;
using System.Runtime.InteropServices;
using Xunit;

namespace Microsoft.EntityFrameworkCore.SqlServer.Facts
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SkipOnNetFrameworkFact: FactAttribute
    {
        public SkipOnNetFrameworkFact()
        {
            if (IsNetFramework())
            {
                Skip = $"Skipped on .NET Framework [Current Version: {RuntimeInformation.FrameworkDescription}]";
            }
        }

        private static bool IsNetFramework()
            => RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase);
    }
}
