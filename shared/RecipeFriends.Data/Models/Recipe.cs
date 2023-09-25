using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RecipeFriends.Shared;

namespace RecipeFriends.Data.Models;

[Table("Recipe")]
public class Recipe
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string Title { get; set; }
    public required Catagories Catagory { get; set; }
    public required string ShortDescription { get; set; }
    public required string Description { get; set; }
    public required string Directions { get; set; }

    public string? Notes { get; set;}

    public required TimeOnly PreparationTime { get; set; }
    public required TimeOnly CookingTime { get; set; }

    // Navigation properties for the many-to-many relationships
    public virtual ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

    public virtual ICollection<ImageData> Images { get; set; } = new List<RecipeFriends.Data.Models.ImageData>();
}
