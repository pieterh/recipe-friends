using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeFriends.Shared.Data.Models;

[Table("Ingredients")]
public class Ingredient
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string Name { get; set; }
    public required double Amount { get; set; }
    public int MeasurementId { get; set; }  // FK for Measurement
    [Required]
    public Measurement MeasurementNew { get; set; }  // Navigation property
    public required int Order { get; set; }
}
