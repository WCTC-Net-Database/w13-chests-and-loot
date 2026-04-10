using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsoleRpgEntities.Migrations
{
    /// <inheritdoc />
    public partial class SeedWorldContent : BaseMigration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Loads Migrations/Scripts/SeedWorldContent.sql and executes it.
            // See BaseMigration.cs for how the script resolution works.
            RunSql(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Loads Migrations/Scripts/SeedWorldContent.rollback.sql and executes it.
            RunSqlRollback(migrationBuilder);
        }
    }
}
