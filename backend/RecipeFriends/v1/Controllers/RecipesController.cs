
using System.Net.Mime;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using RecipeFriends.Data;
using RecipeFriends.Models;
using RecipeFriends.Shared.DTO;

namespace RecipeFriends.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly RecipeFriendsContext _context;

        public RecipesController(RecipeFriendsContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get a list of all recipes with there basic information.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/v1/recipes
        /// </remarks>
        [HttpGet]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponse(200, "Returns list of all recipes.", typeof(IEnumerable<RecipeInfo>))]
        [SwaggerResponse(500, "Unexpected")]
        public ActionResult<IEnumerable<RecipeInfo>> GetRecipes()
        {
            return _context.Recipes.Include(r => r.Tags).ToList().Select(ToRecipeInfoDTO).ToList();
        }

        /// <summary>
        /// Get the details of a specific recipe.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/v1/recipes/5
        /// </remarks>
        [HttpGet("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponse(200, "Returns details of a recipe.", typeof(RecipeDetails))]
        [SwaggerResponse(500, "Unexpected")]
        public ActionResult<RecipeDetails> GetRecipe(int id)
        {
            var recipe = _context.Recipes.Find(id);
            if (recipe == null)
            {
                return NotFound();
            }
            ;
            return ToRecipeDetailsDTO(recipe);
        }

        /// <summary>
        /// Creates a new recipe.
        /// </summary>
        /// <param name="recipe">The model for creating a new recipe.</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/recipes
        ///     {
        ///        "id": 1,
        ///        "title": "Item #1",
        ///        "description": "asd",
        ///        "content": "asd",
        ///        "tags": [
        ///          "Pork"
        ///        ]
        ///     }
        ///
        /// </remarks>
        [HttpPost()]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(201, "Returns the newly created recipe.", typeof(RecipeDetails))]
        [SwaggerResponse(400, "If the recipe is null or not valid.")]
        [SwaggerResponse(500, "Unexpected")]
        public async Task<ActionResult<RecipeDetails>> CreateRecipe(RecipeDetails recipe)
        {
            var recipeModel = ToRecipe(recipe);
            _context.Recipes.Add(recipeModel);
            await _context.SaveChangesAsync();
            var r = _context.Recipes.Include(r => r.Tags).First(r => r.Id == recipeModel.Id);
            return new CreatedAtActionResult(nameof(GetRecipe),
                                "Recipe",
                                new { id = recipeModel.Id },
                                ToRecipeDetailsDTO(r));
        }

        /// <summary>
        /// Update the details of a specific recipe.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT api/v1/recipes/5
        /// </remarks>
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces("application/json")]
        [SwaggerResponse(204, "Updated")]
        [SwaggerResponse(400, "If the recipe is null or not valid.")]
        [SwaggerResponse(404, "If the recipe is not found.")]
        [SwaggerResponse(500, "Unexpected")]
        public async Task<IActionResult> UpdateRecipe(int id, RecipeDetails recipeDTO)
        {
            if (id != recipeDTO.Id)
            {
                return BadRequest("Id mismatch");
            }

            var existingRecipe = await _context.Recipes.Include(r => r.Tags).FirstOrDefaultAsync(r => r.Id == id);

            if (existingRecipe == null)
            {
                return NotFound();
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
                        tagRecipe = new Models.Tag() { Name = tagDTO };
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
                if (!_context.Recipes.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private Recipe ToRecipe(RecipeDetails recipeDTO)
        {
            var recipe = new Recipe
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
                    existingTag = new Tag { Name = tagDTO };
                    _context.Tags.Add(existingTag);
                }

                recipe.Tags.Add(existingTag);
            }

            return recipe;
        }

        private RecipeDetails ToRecipeDetailsDTO(Recipe recipe)
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

        private RecipeInfo ToRecipeInfoDTO(Recipe recipe)
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

        private IngredientDetails ToIngredientDTO(Ingredient ingredient)
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
}
