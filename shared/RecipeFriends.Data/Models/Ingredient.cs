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
    // Change the Measurement enum to Measurement class
    public RecipeFriends.Shared.Measurement Measurement { get; set; }  // Navigation property
    public int MeasurementId { get; set; }  // FK for Measurement
    [Required]
    public Measurement MeasurementNew { get; set; }  // Navigation property
    public required int Order { get; set; }
}
