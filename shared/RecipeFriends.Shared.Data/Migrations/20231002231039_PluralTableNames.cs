using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeFriends.Shared.Data.Migrations
{
    /// <inheritdoc />
    public partial class PluralTableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipeTag_Recipe_RecipesId",
                table: "RecipeTag");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeTag_Tag_TagsId",
                table: "RecipeTag");

            migrationBuilder.DropIndex(
                name: "IX_Image_RecipeId", 
                table: "Recipe");

            migrationBuilder.DropIndex(
                name: "IX_Ingredient_MeasurementId",
                table: "Ingredients");

            migrationBuilder.DropIndex(
                name: "IX_Ingredient_RecipeId",
                table: "Ingredients");

            migrationBuilder.DropIndex(
                name: "IX_Recipe_CategoryId",
                table: "Recipes");   

            migrationBuilder.DropPrimaryKey("PK_Recipe", "Recipe");             
            migrationBuilder.DropPrimaryKey("PK_Tag", "Tag");
            migrationBuilder.DropPrimaryKey("PK_Image", "Image");
            migrationBuilder.DropPrimaryKey("PK_Ingredient", "Ingredient");

            migrationBuilder.RenameTable("Recipe", null, "Recipes", null);
            migrationBuilder.RenameTable("Tag", null, "Tags", null);
            migrationBuilder.RenameTable("Image", null, "Images", null);
            migrationBuilder.RenameTable("Ingredient", null, "Ingredients", null);

            migrationBuilder.AddPrimaryKey("PK_Recipes", "Recipes", "Id", null);
            migrationBuilder.AddPrimaryKey("PK_Tags", "Tags", "Id", null);
            migrationBuilder.AddPrimaryKey("PK_Images", "Images", "Id", null);
            migrationBuilder.AddPrimaryKey("PK_Ingredients", "Ingredients", "Id", null);

            migrationBuilder.CreateIndex(
                name: "IX_Images_RecipeId",
                table: "Images",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_MeasurementId",
                table: "Ingredients",
                column: "MeasurementId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_RecipeId",
                table: "Ingredients",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_CategoryId",
                table: "Recipes",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeTag_Recipes_RecipesId",
                table: "RecipeTag",
                column: "RecipesId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeTag_Tags_TagsId",
                table: "RecipeTag",
                column: "TagsId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipeTag_Recipes_RecipesId",
                table: "RecipeTag");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeTag_Tags_TagsId",
                table: "RecipeTag");

           migrationBuilder.DropIndex(
                name: "IX_Images_RecipeId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_MeasurementId",
                table: "Ingredients");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_RecipeId",
                table: "Ingredients");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_CategoryId",
                table: "Recipes");

            migrationBuilder.DropPrimaryKey("PK_Recipes", "Recipes");
            migrationBuilder.DropPrimaryKey("PK_Tags", "Tags", "Id");
            migrationBuilder.DropPrimaryKey("PK_Images", "Images");
            migrationBuilder.DropPrimaryKey("PK_Ingredients", "Ingredients");

            migrationBuilder.RenameTable("Recipes", null, "Recipe", null);
            migrationBuilder.RenameTable("Tags", null, "Tag", null);
            migrationBuilder.RenameTable("Images", null, "Image", null);
            migrationBuilder.RenameTable("Ingredients", null, "Ingredient", null);

            migrationBuilder.AddPrimaryKey("PK_Recipe", "Recipe", "Id", null);
            migrationBuilder.AddPrimaryKey("PK_Tag", "Tag", "Id", null);
            migrationBuilder.AddPrimaryKey("PK_Image", "Image", "Id", null);
            migrationBuilder.AddPrimaryKey("PK_Ingredient", "Ingredient", "Id", null);

            migrationBuilder.CreateIndex(
                name: "IX_Image_RecipeId",
                table: "Image",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_MeasurementId",
                table: "Ingredient",
                column: "MeasurementId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_RecipeId",
                table: "Ingredient",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_CategoryId",
                table: "Recipe",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeTag_Recipe_RecipesId",
                table: "RecipeTag",
                column: "RecipesId",
                principalTable: "Recipe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeTag_Tag_TagsId",
                table: "RecipeTag",
                column: "TagsId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
