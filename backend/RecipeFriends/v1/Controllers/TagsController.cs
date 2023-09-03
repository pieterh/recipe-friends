
using System.Net.Mime;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using RecipeFriends.Data;
using RecipeFriends.Models;
using RecipeFriends.Shared.DTO.v1;

namespace RecipeFriends.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly RecipeFriendsContext _context;

        public TagsController(RecipeFriendsContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get a list of all tags.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/v1/tags
        /// </remarks>
        [HttpGet]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponse(200, "Returns list of all recipes.", typeof(IEnumerable<RecipeInfo>))]
        [SwaggerResponse(500, "Unexpected")]
        public async Task<ActionResult<IEnumerable<TagInfo>>> GetTags()
        {
            var tags = await _context.Tags.Select(t => new TagInfo
            {
                Id = t.Id,
                Name = t.Name
            }).ToListAsync();

            return Ok(tags);
        }

        /// <summary>
        /// Get the details of a specific tag.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/v1/tags/5
        /// </remarks>
        [HttpGet("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponse(200, "Returns details of a tag.", typeof(TagInfo))]
        [SwaggerResponse(500, "Unexpected.")]
        public async Task<ActionResult<TagInfo>> GetTag(int id)
        {
            var tag = await _context.Tags.FindAsync(id);

            if (tag == null)
            {
                return NotFound();
            }

            return new TagInfo { Id = tag.Id, Name = tag.Name };
        }

        /// <summary>
        /// Creates a new tag.
        /// </summary>
        /// <param name="tag">The model for creating a new tag.</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/tags
        ///     {
        ///        "id": 1,
        ///        "name": "Item #1"
        ///     }
        ///
        /// </remarks>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(201, "Returns the newly created tag.", typeof(TagInfo))]
        [SwaggerResponse(400, "If the tag is null or not valid.")]
        [SwaggerResponse(500, "Unexpected.")]
        public async Task<ActionResult<TagInfo>> CreateTag(TagInfo tag)
        {
            var newTag = new Models.Tag
            {
                Name = tag.Name
            };

            _context.Tags.Add(newTag);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, new TagInfo { Id = tag.Id, Name = tag.Name });
        }


        /// <summary>
        /// Update the details of a specific tag.
        /// </summary>
        /// <param name="id">The id of the tag that needs updating.</param>
        /// <param name="tag">The model for updating an existing tag.</param>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT api/v1/tags/5
        /// </remarks>
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces("application/json")]
        [SwaggerResponse(204, "Updated")]
        [SwaggerResponse(400, "If the tag is null or not valid.")]
        [SwaggerResponse(404, "If the tag is not found.")]
        [SwaggerResponse(500, "Unexpected.")]
        public async Task<IActionResult> UpdateTag(int id, TagInfo tag)
        {
            if (id != tag.Id)
            {
                return BadRequest("Id mismatch");
            }

            var existingTag = await _context.Tags.FindAsync(id);

            if (existingTag == null)
            {
                return NotFound();
            }

            existingTag.Name = tag.Name;

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
    }
}
