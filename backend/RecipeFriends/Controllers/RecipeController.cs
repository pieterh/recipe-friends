using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeFriends.Data;
using RecipeFriends.Shared.DTO;
using RecipeFriends.Model;
using System.Collections.Generic;
using System.Linq;


namespace RecipeFriends.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly RecipeFriendsContext _context;

        public RecipeController(RecipeFriendsContext context)
        {
            _context = context;
        }

        // GET: api/Recipe
        [HttpGet]
        public ActionResult<IEnumerable<Shared.DTO.Recipe>> GetRecipes()
        {
            return _context.Recipes.Include(r => r.Tags).ToList().Select(ToRecipeDTO).ToList();
        }

        // GET: api/Recipe/5
        [HttpGet("{id}")]
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

        // POST: api/Recipe
        [HttpPost]
        public async Task<IActionResult> CreateRecipe(Shared.DTO.Recipe recipeDTO)
        {
            var recipe = ToRecipe(recipeDTO);
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            var r = _context.Recipes.Include(r => r.Tags).First(r => r.Id == recipe.Id);
            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, ToRecipeDTO(r));
        }

        // PUT: api/Recipe/5
        [HttpPut("{id}")]
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
            existingRecipe.Description = recipeDTO.Description;
            existingRecipe.Content = recipeDTO.Content;

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
                        tagRecipe = new Model.Tag() { Name = tagDTO };
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
        public async Task<ActionResult<Shared.DTO.Tag>> CreateTag(Shared.DTO.Tag tagDTO)
        {
            var tag = new Model.Tag
            {
                Name = tagDTO.Name
            };

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, new Shared.DTO.Tag { Id = tag.Id, Name = tag.Name });
        }

        [HttpGet("tags")]
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
        public async Task<ActionResult<Shared.DTO.Tag>> GetTag(int id)
        {
            var tag = await _context.Tags.FindAsync(id);

            if (tag == null)
            {
                return NotFound();
            }

            return new Shared.DTO.Tag { Id = tag.Id, Name = tag.Name };
        }


        // ... [potentially other actions like DELETE, etc.]
        private Model.Recipe ToRecipe(Shared.DTO.Recipe recipeDTO)
        {
            var recipe =  new Model.Recipe
            {
                Id = recipeDTO.Id,
                Title = recipeDTO.Title,
                Description = recipeDTO.Description,
                Content = recipeDTO.Content,
            };

            foreach (var tagDTO in recipeDTO.Tags)
            {
                // Check if the tag already exists in the database                
                var existingTag = _context.Tags.FirstOrDefault((x) => x.Name == tagDTO);
                if (existingTag == null)
                {
                    // The tag doesn't exist, so create it
                    existingTag = new Model.Tag { Name = tagDTO };
                    _context.Tags.Add(existingTag);
                }

                recipe.Tags.Add(existingTag);
            }

            return recipe;
        }

        private Shared.DTO.Recipe ToRecipeDTO(Model.Recipe recipe)
        {
            // make sure the tags are loaded
            _context.Entry(recipe).Collection(r => r.Tags).Load();
            return new Shared.DTO.Recipe
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                Content = recipe.Content,
                Tags = recipe.Tags.Select(rt => rt.Name).ToList()
            };
        }
    }






}


