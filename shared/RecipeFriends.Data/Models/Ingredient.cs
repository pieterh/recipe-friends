using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RecipeFriends.Shared;

namespace RecipeFriends.Data.Models;

[Table("Ingredient")]
public class Ingredient
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string Name { get; set; }
    public required double Amount { get; set; }
    public required Measurement Measurement { get; set; }
    public required int Order { get; set; }
}
