using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeFriends.Shared.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Catagory",
                table: "Recipe");

            migrationBuilder.DropColumn(
                name: "Measurement",
                table: "Ingredient");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Catagory",
                table: "Recipe",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Measurement",
                table: "Ingredient",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
