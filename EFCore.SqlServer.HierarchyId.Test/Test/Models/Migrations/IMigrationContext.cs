namespace Microsoft.EntityFrameworkCore.SqlServer.Test.Models.Migrations
{
    internal interface IMigrationContext
    {
        string GetExpectedMigrationCode(string migrationName, string @namespace);
        string GetExpectedSnapshotCode(string @namespace);
    }
}
