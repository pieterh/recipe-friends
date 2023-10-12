using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeFriends.Shared.Data.Models;

[Table("Measurements")]
public class Measurement
{
    public static Measurement Unset { get { return new Measurement() { MeasurementId = -1, Name = string.Empty }; } }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MeasurementId { get; set; }

    [Required]
    [MaxLength(100)]  // Adjust based on the longest measurement name you expect
    public required string Name { get; set; }
}
