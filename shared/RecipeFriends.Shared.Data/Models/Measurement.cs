using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeFriends.Shared.Data.Models;

[Table("Measurements")]
public class Measurement
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MeasurementId { get; set; }

    [Required]
    [MaxLength(100)]  // Adjust based on the longest measurement name you expect
    public string Name { get; set; }
}
