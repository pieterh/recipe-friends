using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeFriends.Model;

public class Recipe
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string Title { get; set; }
    
    public required string Description { get; set; }
    
    public required string Content { get; set; }

    // Navigation property for the many-to-many relationship
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
