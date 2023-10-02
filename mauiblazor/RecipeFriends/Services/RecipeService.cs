
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.EntityFrameworkCore;
using RecipeFriends.Shared.Data;
using RecipeFriends.Shared.Data.Models;
using RecipeFriends.Shared.DTO;

namespace RecipeFriends.Services;

public class RecipeService : IRecipeService
{
    private readonly RecipeFriendsContext _context;
    
    public RecipeService(RecipeFriendsContext context)
    {
        _context = context;
    }
        
    public async Task<RecipeDetails> GetRecipeDetailsAsync(int id, CancellationToken cancellationToken)
    {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return null;
            }
            ;
            return ToRecipeDetailsDTO(recipe);
    }

    public async Task<RecipeInfo[]> GetRecipesAsync(CancellationToken cancellationToken)
    {
            var l = await _context.Recipes.Include(r => r.Tags).ToListAsync();
            var r = l.Select(ToRecipeInfoDTO).ToArray();
            return r;
    }

    public async Task<bool> SaveRecipeDetailsAsync(RecipeDetails recipeDTO, CancellationToken cancellationToken)
    {     
        if (recipeDTO.Id > 0) {
            var existingRecipe = await _context.Recipes.Include(r => r.Tags).FirstOrDefaultAsync(r => r.Id == recipeDTO.Id, cancellationToken: cancellationToken);

            if (existingRecipe == null)
            {
                return false;
            }

            // Update basic properties
            existingRecipe.Title = recipeDTO.Title;
            existingRecipe.CategoryId = recipeDTO.Category.Id;
            existingRecipe.ShortDescription = recipeDTO.ShortDescription;
            existingRecipe.Description = recipeDTO.Description;
            existingRecipe.Directions = recipeDTO.Directions;
            existingRecipe.PreparationTime = recipeDTO.PreparationTime;
            existingRecipe.CookingTime = recipeDTO.CookingTime;

            // Handle tags
            // Identify tags that are no longer associated
            var tagsToRemove = existingRecipe.Tags
                                                .Where(rt => !recipeDTO.Tags.Any(t => t.Id == rt.Id))
                                                .ToList();

            foreach (var tagToRemove in tagsToRemove)
            {
                existingRecipe.Tags.Remove(tagToRemove);
            }

            // Add new tags
            foreach (var tagDTO in recipeDTO.Tags)
            {
                // Check if the tag already exists in the database
                if (!existingRecipe.Tags.Any(rt => rt.Id == tagDTO.Id))
                {
                    var tagRecipe = _context.Tags.FirstOrDefault((x) => x.Id == tagDTO.Id);
                    if (tagRecipe == null)
                    {
                        tagRecipe = new Tag() { Name = tagDTO.Name };
                        _context.Tags.Add(tagRecipe);
                    }
                    existingRecipe.Tags.Add(tagRecipe);
                }
            }
        }
        else{
            var newRecipe = new Recipe(){
                Title = recipeDTO.Title,
                CategoryId = recipeDTO.Category.Id,
                ShortDescription = recipeDTO.ShortDescription,
                Description = recipeDTO.Description,
                Directions = recipeDTO.Directions,
                PreparationTime = recipeDTO.PreparationTime,
                CookingTime = recipeDTO.CookingTime
            };
            _context.Add(newRecipe);
        }

        // Save changes
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // This checks if the resource still exists in the database.
            if (!_context.Recipes.Any(e => e.Id == recipeDTO.Id))
            {
                return false;
            }
            else
            {
                throw;
            }
        }

        return true;
    }

    public async Task<TagInfo[]> GetTagsAsync(CancellationToken cancellationToken){
        var tags = await _context.Tags.ToListAsync(cancellationToken: cancellationToken);
        return tags.Select(ToTagDTO).ToArray();
    }

    public async Task<CategoryInfo[]> GetCategoriesAsync(CancellationToken cancellationToken){
        var categories = await _context.Catagories.Where(c => c.Status == EntityStatus.Active).ToListAsync(cancellationToken: cancellationToken);
        return categories.Select(ToCategoryDTO).ToArray();
    }

    public async Task<MeasurementInfo[]> GetMeasurementsAsync(CancellationToken cancellationToken){
        var measurements = await _context.Measurements.ToListAsync(cancellationToken: cancellationToken);
        return measurements.Select(ToMeasurementDTO).ToArray();
    }

           private Recipe ToRecipe(RecipeDetails recipeDTO)
        {
            var recipe = new Recipe
            {
                Id = recipeDTO.Id,
                Title = recipeDTO.Title,
                CategoryId = recipeDTO.Category.Id,
                ShortDescription = recipeDTO.ShortDescription,
                Description = recipeDTO.Description,
                Directions = recipeDTO.Directions,
                PreparationTime = recipeDTO.PreparationTime,
                CookingTime = recipeDTO.CookingTime
            };

            foreach (var tagDTO in recipeDTO.Tags)
            {
                // Check if the tag already exists in the database                
                var existingTag = _context.Tags.FirstOrDefault((x) => x.Id == tagDTO.Id);
                if (existingTag == null)
                {
                    // The tag doesn't exist, so create it
                    existingTag = new Tag { Name = tagDTO.Name };
                    _context.Tags.Add(existingTag);
                }

                recipe.Tags.Add(existingTag);
            }

            return recipe;
        }



        private RecipeDetails ToRecipeDetailsDTO(Recipe recipe)
        {
            // make sure the related lists and references are loaded
            _context.Entry(recipe).Collection(r => r.Ingredients).Load();
            _context.Entry(recipe).Collection(r => r.Tags).Load();
            _context.Entry(recipe).Reference(r => r.Category).Load();
            return new RecipeDetails
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Category = ToCategoryDTO(recipe.Category),
                ShortDescription = recipe.ShortDescription,
                Description = recipe.Description,
                Directions = recipe.Directions,
                PreparationTime = recipe.PreparationTime,
                CookingTime = recipe.CookingTime,
                Ingredients = recipe.Ingredients.Select(ToIngredientDTO).ToList(),
                Tags = recipe.Tags.Select(rt => new TagInfo() { Id = rt.Id, Name = rt.Name}).ToList(),
                Images = recipe.Images.Select(rt => new ImageInfo() { Id = rt.Id, Title = rt.Title, Name = rt.Name }).ToList()
            };
        }

        private RecipeInfo ToRecipeInfoDTO(Recipe recipe)
        {
            // make sure the related lists and references are loaded
            _context.Entry(recipe).Collection(r => r.Tags).Load();
            _context.Entry(recipe).Reference(r => r.Category).Load();
            return new RecipeInfo
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Category = new CategoryInfo() {Id = recipe.Category.CategoryId, Name = recipe.Category.Name},
                ShortDescription = recipe.ShortDescription,
                Tags = recipe.Tags.Select(rt => rt.Name).ToList()
            };
        }

        private IngredientDetails ToIngredientDTO(Ingredient ingredient)
        {
            _context.Entry(ingredient).Reference(r => r.MeasurementNew).Load();
            return new IngredientDetails
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Amount = ingredient.Amount,
                Measurement = ToMeasurementDTO(ingredient.MeasurementNew),
                Order = ingredient.Order
            };
        }

        private TagInfo ToTagDTO(Tag tag){
            return new TagInfo{
                Id = tag.Id,
                Name = tag.Name
            };
        }

        private CategoryInfo ToCategoryDTO(Category category){
            return new CategoryInfo{
                Id = category.CategoryId,
                Name = category.Name
            };
        }

        private MeasurementInfo ToMeasurementDTO(Measurement measurement){
            return new MeasurementInfo{
                Id = measurement.MeasurementId,
                Name = measurement.Name
            };
        }
}
