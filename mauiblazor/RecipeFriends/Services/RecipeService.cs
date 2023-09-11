
using Microsoft.EntityFrameworkCore;
using RecipeFriends.Data;
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
        //    if (id != recipeDTO.Id)
        //     {
        //         return BadRequest("Id mismatch");
        //     }

            var existingRecipe = await _context.Recipes.Include(r => r.Tags).FirstOrDefaultAsync(r => r.Id == recipeDTO.Id);

            if (existingRecipe == null)
            {
                return false;
            }

            // Update basic properties
            existingRecipe.Title = recipeDTO.Title;
            existingRecipe.Catagory = recipeDTO.Catagory;
            existingRecipe.ShortDescription = recipeDTO.ShortDescription;
            existingRecipe.Description = recipeDTO.Description;
            existingRecipe.Directions = recipeDTO.Directions;
            existingRecipe.PreparationTime = recipeDTO.PreparationTime;
            existingRecipe.CookingTime = recipeDTO.CookingTime;

            // Handle tags
            // Identify tags that are no longer associated
            var tagsToRemove = existingRecipe.Tags
                                             .Where(rt => !recipeDTO.Tags.Any(t => t == rt.Name))
                                             .ToList();

            foreach (var tagToRemove in tagsToRemove)
            {
                existingRecipe.Tags.Remove(tagToRemove);
            }

            // Add new tags
            foreach (var tagDTO in recipeDTO.Tags)
            {
                // Check if the tag already exists in the database
                if (!existingRecipe.Tags.Any(rt => rt.Name == tagDTO))
                {
                    var tagRecipe = _context.Tags.FirstOrDefault((x) => x.Name == tagDTO);
                    if (tagRecipe == null)
                    {
                        tagRecipe = new Data.Models.Tag() { Name = tagDTO };
                        _context.Tags.Add(tagRecipe);
                    }
                    existingRecipe.Tags.Add(tagRecipe);
                }
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

           private Data.Models.Recipe ToRecipe(RecipeDetails recipeDTO)
        {
            var recipe = new Data.Models.Recipe
            {
                Id = recipeDTO.Id,
                Title = recipeDTO.Title,
                Catagory = recipeDTO.Catagory,
                ShortDescription = recipeDTO.ShortDescription,
                Description = recipeDTO.Description,
                Directions = recipeDTO.Directions,
                PreparationTime = recipeDTO.PreparationTime,
                CookingTime = recipeDTO.CookingTime
            };

            foreach (var tagDTO in recipeDTO.Tags)
            {
                // Check if the tag already exists in the database                
                var existingTag = _context.Tags.FirstOrDefault((x) => x.Name == tagDTO);
                if (existingTag == null)
                {
                    // The tag doesn't exist, so create it
                    existingTag = new Data.Models.Tag { Name = tagDTO };
                    _context.Tags.Add(existingTag);
                }

                recipe.Tags.Add(existingTag);
            }

            return recipe;
        }

        private RecipeDetails ToRecipeDetailsDTO(Data.Models.Recipe recipe)
        {
            // make sure the related lists are loaded
            _context.Entry(recipe).Collection(r => r.Ingredients).Load();
            _context.Entry(recipe).Collection(r => r.Tags).Load();

            return new RecipeDetails
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Catagory = recipe.Catagory,
                ShortDescription = recipe.ShortDescription,
                Description = recipe.Description,
                Directions = recipe.Directions,
                PreparationTime = recipe.PreparationTime,
                CookingTime = recipe.CookingTime,
                Ingredients = recipe.Ingredients.Select(ToIngredientDTO).ToList(),
                Tags = recipe.Tags.Select(rt => rt.Name).ToList()
            };
        }

        private RecipeInfo ToRecipeInfoDTO(Data.Models.Recipe recipe)
        {
            // make sure the related lists are loaded
            _context.Entry(recipe).Collection(r => r.Tags).Load();

            return new RecipeInfo
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Catagory = recipe.Catagory,
                ShortDescription = recipe.ShortDescription,
                Tags = recipe.Tags.Select(rt => rt.Name).ToList()
            };
        }

        private IngredientDetails ToIngredientDTO(Data.Models.Ingredient ingredient)
        {
            return new IngredientDetails
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Amount = ingredient.Amount,
                Measurement = ingredient.Measurement,
                Order = ingredient.Order
            };
        }
}
