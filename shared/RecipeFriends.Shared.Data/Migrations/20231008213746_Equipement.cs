using Microsoft.EntityFrameworkCore.Migrations;
using RecipeFriends.Shared.Data.Models;

#nullable disable

namespace RecipeFriends.Shared.Data.Migrations
{
    /// <inheritdoc />
    public partial class Equipement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentRecipe",
                columns: table => new
                {
                    EquipmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    RecipesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentRecipe", x => new { x.EquipmentId, x.RecipesId });
                    table.ForeignKey(
                        name: "FK_EquipmentRecipe_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentRecipe_Recipes_RecipesId",
                        column: x => x.RecipesId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentRecipe_RecipesId",
                table: "EquipmentRecipe",
                column: "RecipesId");

            migrationBuilder.InsertData(
                table: "Equipment",
                columns: new[] { "Id", "Name", "Status" },
                values: new object[,]
                {
                    { 1, "Skillet", (int)EntityStatus.Active },
                    { 2, "Dutch oven", (int)EntityStatus.Active },
                    { 3, "Soapstone", (int)EntityStatus.Active },
                    { 4, "Cold smoke generator", (int)EntityStatus.Active },
                    { 5, "Bricknic", (int)EntityStatus.Active }       
                }
            );                
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentRecipe");

            migrationBuilder.DropTable(
                name: "Equipment");
        }
    }
}
