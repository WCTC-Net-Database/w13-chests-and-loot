using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsoleRpgEntities.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentSlots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add the new EligibleSlot column (nullable int = SlotType?).
            migrationBuilder.AddColumn<int>(
                name: "EligibleSlot",
                table: "Items",
                type: "int",
                nullable: true);

            // 2. Migrate data from the legacy Armor.Slot string into EligibleSlot.
            //    SlotType enum values: Head=0, Body=1, Legs=2, Feet=3, Hands=4,
            //    Weapon=5, Shield=6, Ring=7, Accessory=8
            migrationBuilder.Sql(@"
                UPDATE Items SET EligibleSlot = 0 WHERE ItemType = 'Armor'  AND Slot = 'Head';
                UPDATE Items SET EligibleSlot = 1 WHERE ItemType = 'Armor'  AND Slot = 'Body';
                UPDATE Items SET EligibleSlot = 2 WHERE ItemType = 'Armor'  AND Slot = 'Legs';
                UPDATE Items SET EligibleSlot = 3 WHERE ItemType = 'Armor'  AND Slot = 'Feet';
                UPDATE Items SET EligibleSlot = 4 WHERE ItemType = 'Armor'  AND Slot = 'Hands';
                UPDATE Items SET EligibleSlot = 6 WHERE ItemType = 'Armor'  AND Slot = 'Shield';
                UPDATE Items SET EligibleSlot = 7 WHERE ItemType = 'Armor'  AND Slot = 'Ring';
                UPDATE Items SET EligibleSlot = 8 WHERE ItemType = 'Armor'  AND Slot = 'Accessory';
                UPDATE Items SET EligibleSlot = 5 WHERE ItemType = 'Weapon';
            ");

            // 3. Drop the legacy Slot string column now that data is migrated.
            migrationBuilder.DropColumn(
                name: "Slot",
                table: "Items");

            // 4. Create the EquipmentSlots table.
            migrationBuilder.CreateTable(
                name: "EquipmentSlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SlotType = table.Column<int>(type: "int", nullable: false),
                    EquippedItemId = table.Column<int>(type: "int", nullable: true),
                    EquipmentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentSlots_Containers_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Containers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentSlots_Items_EquippedItemId",
                        column: x => x.EquippedItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSlots_EquipmentId",
                table: "EquipmentSlots",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSlots_EquippedItemId",
                table: "EquipmentSlots",
                column: "EquippedItemId");

            // 5. Seed default slots for every existing Equipment container.
            //    One row per SlotType per Equipment row, all empty (EquippedItemId = NULL).
            //    Works for any number of pre-existing Equipment containers.
            migrationBuilder.Sql(@"
                INSERT INTO EquipmentSlots (SlotType, EquippedItemId, EquipmentId)
                SELECT s.SlotType, NULL, c.Id
                FROM Containers c
                CROSS JOIN (VALUES (0),(1),(2),(3),(4),(5),(6),(7),(8)) AS s(SlotType)
                WHERE c.ContainerType = 'Equipment';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentSlots");

            migrationBuilder.AddColumn<string>(
                name: "Slot",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            // Best-effort restore of the Slot string from EligibleSlot.
            migrationBuilder.Sql(@"
                UPDATE Items SET Slot = 'Head'      WHERE ItemType = 'Armor' AND EligibleSlot = 0;
                UPDATE Items SET Slot = 'Body'      WHERE ItemType = 'Armor' AND EligibleSlot = 1;
                UPDATE Items SET Slot = 'Legs'      WHERE ItemType = 'Armor' AND EligibleSlot = 2;
                UPDATE Items SET Slot = 'Feet'      WHERE ItemType = 'Armor' AND EligibleSlot = 3;
                UPDATE Items SET Slot = 'Hands'     WHERE ItemType = 'Armor' AND EligibleSlot = 4;
                UPDATE Items SET Slot = 'Shield'    WHERE ItemType = 'Armor' AND EligibleSlot = 6;
                UPDATE Items SET Slot = 'Ring'      WHERE ItemType = 'Armor' AND EligibleSlot = 7;
                UPDATE Items SET Slot = 'Accessory' WHERE ItemType = 'Armor' AND EligibleSlot = 8;
            ");

            migrationBuilder.DropColumn(
                name: "EligibleSlot",
                table: "Items");
        }
    }
}
