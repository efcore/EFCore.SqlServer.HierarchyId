using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Microsoft.EntityFrameworkCore.SqlServer.Migrations
{
    /// <summary>
    /// for .net framework, fixes an issue
    /// where the entry assembly is null.
    /// 
    /// see workaround @ https://github.com/microsoft/vstest/issues/649
    /// see https://github.com/khellang/Scrutor/issues/109
    /// </summary>
    internal static class AssemblyUtils
    {
        public static bool WasEntryAssemblyUpdated { get; private set; }

        public static void SetEntryAssembly()
        {
#if NETFRAMEWORK
            if (Assembly.GetEntryAssembly() == null)
            {
                WasEntryAssemblyUpdated = true;
                UpdateEntryAssembly(Assembly.GetCallingAssembly());
            }
#endif
        }


        private static void UpdateEntryAssembly(Assembly assembly)
        {
#if NETFRAMEWORK
            var manager = new AppDomainManager();
            FieldInfo entryAssemblyfield = manager.GetType().GetField("m_entryAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
            entryAssemblyfield.SetValue(manager, assembly);

            var domain = AppDomain.CurrentDomain;
            FieldInfo domainManagerField = domain.GetType().GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);
            domainManagerField.SetValue(domain, manager);
#endif
        }
    }
}
