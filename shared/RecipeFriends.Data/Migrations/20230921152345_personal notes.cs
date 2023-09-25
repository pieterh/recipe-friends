using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeFriends.Data.Migrations
{
    /// <inheritdoc />
    public partial class personalnotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Recipe",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Recipe");
        }
    }
}
