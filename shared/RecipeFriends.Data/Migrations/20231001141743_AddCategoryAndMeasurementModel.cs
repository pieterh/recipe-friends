using Microsoft.EntityFrameworkCore.Migrations;
using RecipeFriends.Data.Models;
#nullable disable

namespace RecipeFriends.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryAndMeasurementModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Recipe",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MeasurementId",
                table: "Ingredient",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LanguageCode = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    LanguageName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.LanguageId);
                });

            migrationBuilder.CreateTable(
                name: "Measurements",
                columns: table => new
                {
                    MeasurementId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Measurements", x => x.MeasurementId);
                });

            migrationBuilder.CreateTable(
                name: "CategoryTranslations",
                columns: table => new
                {
                    CategoryTranslationId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    LanguageId = table.Column<int>(type: "INTEGER", nullable: false),
                    TranslatedName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryTranslations", x => x.CategoryTranslationId);
                    table.ForeignKey(
                        name: "FK_CategoryTranslations_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryTranslations_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeasurementTranslations",
                columns: table => new
                {
                    MeasurementTranslationId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MeasurementId = table.Column<int>(type: "INTEGER", nullable: false),
                    LanguageId = table.Column<int>(type: "INTEGER", nullable: false),
                    TranslatedName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementTranslations", x => x.MeasurementTranslationId);
                    table.ForeignKey(
                        name: "FK_MeasurementTranslations_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeasurementTranslations_Measurements_MeasurementId",
                        column: x => x.MeasurementId,
                        principalTable: "Measurements",
                        principalColumn: "MeasurementId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_CategoryId",
                table: "Recipe",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_MeasurementId",
                table: "Ingredient",
                column: "MeasurementId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryTranslations_CategoryId",
                table: "CategoryTranslations",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryTranslations_LanguageId",
                table: "CategoryTranslations",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementTranslations_LanguageId",
                table: "MeasurementTranslations",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementTranslations_MeasurementId",
                table: "MeasurementTranslations",
                column: "MeasurementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredient_Measurements_MeasurementId",
                table: "Ingredient",
                column: "MeasurementId",
                principalTable: "Measurements",
                principalColumn: "MeasurementId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipe_Categories_CategoryId",
                table: "Recipe",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.InsertData(
                table:  "Languages",
                columns: new[] { "LanguageId", "LanguageCode", "LanguageName", "Status" },
                values: new object[,]
                {
                    { 1, "nl-NL", "Dutch", (int)EntityStatus.Active },
                    { 2, "en-US", "English", (int)EntityStatus.Active },
                    { 3, "de-DE", "German", (int)EntityStatus.Active },
                    { 4, "fr-FR", "French", (int)EntityStatus.Active },
                    { 5, "es-ES", "Spanish", (int)EntityStatus.Active },
                    { 6, "it-IT", "Italian", (int)EntityStatus.Active }
                }
            );

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "Name", "Status" },
                values: new object[,]
                {
                    { 1, "Amuse Bouche", (int)EntityStatus.Active },
                    { 2, "Appetiser", (int)EntityStatus.Active },
                    { 3, "Soup", (int)EntityStatus.Active },
                    { 4, "Salad", (int)EntityStatus.Active },
                    { 5, "Main Course", (int)EntityStatus.Active },
                    { 6, "Dessert", (int)EntityStatus.Active },
                    { 7, "Sauce", (int)EntityStatus.Active },
                    { 8, "Drink", (int)EntityStatus.Active }            
                }
            );

            migrationBuilder.InsertData(
                table: "Measurements",
                columns: new[] { "MeasurementId", "Name" },
                values: new object[,]
                {
                    { 1, "#" },
                    { 2, "milligram" },
                    { 3, "gram" },
                    { 4, "kilogram" },
                    { 5, "liter" },
                    { 6, "deciliter" },
                    { 7, "milliliter" },
                    { 8, "teaspoon" },
                    { 9, "tablespoon" },
                    { 10, "cup" },
                    { 11, "ounce" },
                    { 12, "pound" }
                });

            migrationBuilder.InsertData(
                table: "MeasurementTranslations",
                columns: new[] { "MeasurementTranslationId", "MeasurementId", "LanguageId", "TranslatedName" },
                values: new object[,]
                {
                    { 1, 1, 1, "#" },
                    { 2, 2, 1, "milligram" },
                    { 3, 3, 1, "gram" },
                    { 4, 4, 1, "kilogram" },
                    { 5, 5, 1, "liter" },
                    { 6, 6, 1, "deciliter" },
                    { 7, 6, 1, "milliliter" },
                    { 8, 7, 1, "theelepel" },
                    { 9, 8, 1, "eetlepel" },
                    { 10, 9, 1, "kopje" },
                    { 11, 10, 1, "ounce" },
                    { 12, 1, 1, "pond" }
                });

            migrationBuilder.InsertData(
                table: "CategoryTranslations",
                columns: new[] { "CategoryTranslationId", "CategoryId", "LanguageId", "TranslatedName" },
                values: new object[,]
                {
                    { 1, 1, 1, "Amuse" },
                    { 2, 2, 1, "Voorgerecht" },
                    { 3, 3, 1, "Soep" },
                    { 4, 4, 1, "Salade" },
                    { 5, 5, 1, "Hoofdgerecht" },
                    { 6, 6, 1, "Nagerecht" },
                    { 7, 7, 1, "Saus" },
                    { 8, 8, 1, "Drank" }
                });

            // Update the Recipe records by mapping enum values to CategoryId values
            migrationBuilder.Sql("UPDATE Recipe SET CategoryId = 1 WHERE Catagory = 10");  // Map AmuseBouche
            migrationBuilder.Sql("UPDATE Recipe SET CategoryId = 2 WHERE Catagory = 20");  
            migrationBuilder.Sql("UPDATE Recipe SET CategoryId = 3 WHERE Catagory = 30");  
            migrationBuilder.Sql("UPDATE Recipe SET CategoryId = 4 WHERE Catagory = 40");  
            migrationBuilder.Sql("UPDATE Recipe SET CategoryId = 5 WHERE Catagory = 50");  
            migrationBuilder.Sql("UPDATE Recipe SET CategoryId = 6 WHERE Catagory = 60");  
            migrationBuilder.Sql("UPDATE Recipe SET CategoryId = 7 WHERE Catagory = 70");  
  
            migrationBuilder.Sql("UPDATE Ingredient SET MeasurementId = 9 WHERE Measurement = 10");
            migrationBuilder.Sql("UPDATE Ingredient SET MeasurementId = 8 WHERE Measurement = 20");
            migrationBuilder.Sql("UPDATE Ingredient SET MeasurementId = 7 WHERE Measurement = 30");
            migrationBuilder.Sql("UPDATE Ingredient SET MeasurementId = 6 WHERE Measurement = 40");
            migrationBuilder.Sql("UPDATE Ingredient SET MeasurementId = 5 WHERE Measurement = 50");
            migrationBuilder.Sql("UPDATE Ingredient SET MeasurementId = 2 WHERE Measurement = 60");
            migrationBuilder.Sql("UPDATE Ingredient SET MeasurementId = 3 WHERE Measurement = 70");
            migrationBuilder.Sql("UPDATE Ingredient SET MeasurementId = 1 WHERE Measurement = 80");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredient_Measurements_MeasurementId",
                table: "Ingredient");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipe_Categories_CategoryId",
                table: "Recipe");

            migrationBuilder.DropTable(
                name: "CategoryTranslations");

            migrationBuilder.DropTable(
                name: "MeasurementTranslations");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "Measurements");

            migrationBuilder.DropIndex(
                name: "IX_Recipe_CategoryId",
                table: "Recipe");

            migrationBuilder.DropIndex(
                name: "IX_Ingredient_MeasurementId",
                table: "Ingredient");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Recipe");

            migrationBuilder.DropColumn(
                name: "MeasurementId",
                table: "Ingredient");
        }
    }
}
