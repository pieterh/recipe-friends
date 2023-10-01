using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeFriends.Data.Models;

[Table("CategoryTranslations")]
public class CategoryTranslation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CategoryTranslationId { get; set; }

    [Required]
    [ForeignKey("Category")]
    public int CategoryId { get; set; }

    public virtual Category Category { get; set; }  // Navigation property

    [Required]
    [ForeignKey("Language")]
    public int LanguageId { get; set; }

    public virtual Language Language { get; set; }  // Navigation property

    [Required]
    [MaxLength(100)]  // Adjust based on the longest translated category name you expect
    public string TranslatedName { get; set; }
}

