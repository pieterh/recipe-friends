using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeFriends.Shared.Data.Models;

[Table("CategoryTranslations")]
public class CategoryTranslation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CategoryTranslationId { get; set; }

    [Required]
    [ForeignKey("Category")]
    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } = Category.Unset;  // Navigation property

    [Required]
    [ForeignKey("Language")]
    public int LanguageId { get; set; }

    public virtual Language Language { get; set; }  = Language.Default;  // Navigation property

    [Required]
    [MaxLength(100)]  // Adjust based on the longest translated category name you expect
    public required string TranslatedName { get; set; }
}

