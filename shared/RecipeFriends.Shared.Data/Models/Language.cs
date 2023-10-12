using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeFriends.Shared.Data.Models;

[Table("Languages")]
public class Language
{
    public static Language Default { get { return new Language() { LanguageId = -1, LanguageName = "US English", LanguageCode = "en-US" }; } }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LanguageId { get; set; }

    [Required]
    [MaxLength(10)]  // This length should cover most language codes, e.g., "en-US"
    public required string LanguageCode { get; set; }

    [Required]
    [MaxLength(50)]  // Adjust based on the longest language name you expect
    public required string LanguageName { get; set; }

    [Required]
    public EntityStatus Status { get; set; }
}
