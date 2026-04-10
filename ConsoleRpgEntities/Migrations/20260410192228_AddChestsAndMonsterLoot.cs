using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsoleRpgEntities.Migrations
{
    /// <inheritdoc />
    public partial class AddChestsAndMonsterLoot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLooted",
                table: "Monsters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LootId",
                table: "Monsters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Containers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "Containers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPickable",
                table: "Containers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTrapped",
                table: "Containers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequiredKeyId",
                table: "Containers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrapDamage",
                table: "Containers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TrapDisarmed",
                table: "Containers",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Monsters_LootId",
                table: "Monsters",
                column: "LootId");

            migrationBuilder.AddForeignKey(
                name: "FK_Monsters_Containers_LootId",
                table: "Monsters",
                column: "LootId",
                principalTable: "Containers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Monsters_Containers_LootId",
                table: "Monsters");

            migrationBuilder.DropIndex(
                name: "IX_Monsters_LootId",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "IsLooted",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "LootId",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "IsPickable",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "IsTrapped",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "RequiredKeyId",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "TrapDamage",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "TrapDisarmed",
                table: "Containers");
        }
    }
}
