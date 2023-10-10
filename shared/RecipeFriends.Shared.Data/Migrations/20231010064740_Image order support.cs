using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeFriends.Shared.Data.Migrations
{
    /// <inheritdoc />
    public partial class Imageordersupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Images",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Images");
        }
    }
}
