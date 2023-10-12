using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeFriends.Shared.Data.Models;

[Table("Categories")]
public class Category
{
    public static Category Unset { get { return new Category() { CategoryId = -1, Name = string.Empty }; } }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CategoryId { get; set; }

    [Required]
    [MaxLength(100)]  // Adjust based on the longest category name you expect
    public required string Name { get; set; }

    [Required]
    public EntityStatus Status { get; set; }
}
