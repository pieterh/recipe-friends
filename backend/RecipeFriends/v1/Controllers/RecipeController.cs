using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeFriends.Data;
using RecipeFriends.Shared.DTO;
using RecipeFriends.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Castle.Core.Resource;
using Swashbuckle.AspNetCore.Annotations;
using Asp.Versioning;

namespace RecipeFriends.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class RecipeController : ControllerBase
    {
        private readonly RecipeFriendsContext _context;

        public RecipeController(RecipeFriendsContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get a list of all recipes
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/v1/Recipe
        /// </remarks>
        [HttpGet]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponse(200, "Returns list of all recipes.", typeof(IEnumerable<Shared.DTO.Recipe>))]
        [SwaggerResponse(500, "Unexpected")]
        public ActionResult<IEnumerable<Shared.DTO.Recipe>> GetRecipes()
        {
            return _context.Recipes.Include(r => r.Tags).ToList().Select(ToRecipeDTO).ToList();
        }

        /// <summary>
        /// Get the details of a specific recipe
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/v1/Recipe/5
        /// </remarks>
        [HttpGet("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponse(200, "Returns list of all recipes.", typeof(Shared.DTO.Recipe))]
        [SwaggerResponse(500, "Unexpected")]
        public ActionResult<Shared.DTO.Recipe> GetRecipe(int id)
        {
            var recipe = _context.Recipes.Find(id);
            if (recipe == null)
            {
                return NotFound();
            }
            ;
            return ToRecipeDTO(recipe);
        }

        /// <summary>
        /// Creates a new recipe.
        /// </summary>
        /// <param name="recipe">The model for creating a new recipe.</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/Recipe
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
        [SwaggerResponse(201, "Returns the newly created recipe.", typeof(Shared.DTO.Recipe))]
        [SwaggerResponse(400, "If the recipe is null or not valid.")]
        [SwaggerResponse(500, "Unexpected")]
        public async Task<ActionResult<Shared.DTO.Recipe>> CreateRecipe(Shared.DTO.Recipe recipe)
        {
            var recipeModel = ToRecipe(recipe);
            _context.Recipes.Add(recipeModel);
            await _context.SaveChangesAsync();
            var r = _context.Recipes.Include(r => r.Tags).First(r => r.Id == recipeModel.Id);
            return new CreatedAtActionResult(nameof(GetRecipe),
                                "Recipe",
                                new { id = recipeModel.Id },
                                ToRecipeDTO(r));
        }

        /// <summary>
        /// Update the details of a specific recipe
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT api/v1/Recipe/5
        /// </remarks>
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces("application/json")]
        [SwaggerResponse(204, "Updated")]
        [SwaggerResponse(400, "If the recipe is null or not valid.")]
        [SwaggerResponse(404, "If the recipe is not found.")]
        [SwaggerResponse(500, "Unexpected")]
        public async Task<IActionResult> UpdateRecipe(int id, Shared.DTO.Recipe recipeDTO)
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

        [HttpPost("tags")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces("application/json")]
        public async Task<ActionResult<Shared.DTO.Tag>> CreateTag(Shared.DTO.Tag tagDTO)
        {
            var tag = new Models.Tag
            {
                Name = tagDTO.Name
            };

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, new Shared.DTO.Tag { Id = tag.Id, Name = tag.Name });
        }

        [HttpGet("tags")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces("application/json")]
        public async Task<ActionResult<IEnumerable<Shared.DTO.Tag>>> GetTags()
        {
            var tags = await _context.Tags.Select(t => new Shared.DTO.Tag
            {
                Id = t.Id,
                Name = t.Name
            }).ToListAsync();

            return Ok(tags);
        }

        [HttpPut("tags/{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateTag(int id, Shared.DTO.Tag tagDTO)
        {
            if (id != tagDTO.Id)
            {
                return BadRequest("Id mismatch");
            }

            var existingTag = await _context.Tags.FindAsync(id);

            if (existingTag == null)
            {
                return NotFound();
            }

            existingTag.Name = tagDTO.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Tags.Any(e => e.Id == id))
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

        [HttpGet("tags/{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces("application/json")]
        public async Task<ActionResult<Shared.DTO.Tag>> GetTag(int id)
        {
            var tag = await _context.Tags.FindAsync(id);

            if (tag == null)
            {
                return NotFound();
            }

            return new Shared.DTO.Tag { Id = tag.Id, Name = tag.Name };
        }

        private Models.Recipe ToRecipe(Shared.DTO.Recipe recipeDTO)
        {
            var recipe = new Models.Recipe
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
                    existingTag = new Models.Tag { Name = tagDTO };
                    _context.Tags.Add(existingTag);
                }

                recipe.Tags.Add(existingTag);
            }

            return recipe;
        }

        private Shared.DTO.Recipe ToRecipeDTO(Models.Recipe recipe)
        {
            // make sure the tags are loaded
            _context.Entry(recipe).Collection(r => r.Tags).Load();
            return new Shared.DTO.Recipe
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Catagory = recipe.Catagory,
                ShortDescription = recipe.ShortDescription,
                Description = recipe.Description,
                Directions = recipe.Directions,
                PreparationTime = recipe.PreparationTime,
                CookingTime = recipe.CookingTime,
                Tags = recipe.Tags.Select(rt => rt.Name).ToList()
            };
        }
    }
}


