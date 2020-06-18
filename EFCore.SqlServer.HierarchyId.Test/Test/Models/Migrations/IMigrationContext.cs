namespace Microsoft.EntityFrameworkCore.SqlServer.Test.Models.Migrations
{
    internal interface IMigrationContext
    {
        public abstract string GetExpectedMigrationCode(string migrationName, string @namespace);
        public abstract string GetExpectedSnapshotCode(string @namespace);
    }
}
