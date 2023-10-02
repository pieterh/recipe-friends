using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeFriends.Shared.Data.Models;

[Table("Categories")]
public class Category
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CategoryId { get; set; }

    [Required]
    [MaxLength(100)]  // Adjust based on the longest category name you expect
    public string Name { get; set; }

    [Required]
    public EntityStatus Status { get; set; }
}
