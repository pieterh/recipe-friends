using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeFriends.Shared.Data.Models;

[Table("Equipment")]
public class Equipment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public required string Name { get; set; }

    [Required]
    public EntityStatus Status { get; set; }

    // Navigation property for the many-to-many relationship
    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
