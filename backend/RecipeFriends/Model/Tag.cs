using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeFriends.Model;

public class Tag
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public required string Name { get; set; }

    // Navigation property for the many-to-many relationship
    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
