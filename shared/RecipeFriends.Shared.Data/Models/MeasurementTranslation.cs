using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeFriends.Shared.Data.Models;

    [Table("MeasurementTranslations")]
    public class MeasurementTranslation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MeasurementTranslationId { get; set; }

        [Required]
        [ForeignKey("Measurement")]
        public int MeasurementId { get; set; }

        public virtual Measurement Measurement { get; set; }  // Navigation property

        [Required]
        [ForeignKey("Language")]
        public int LanguageId { get; set; }

        public virtual Language Language { get; set; }  // Navigation property

        [Required]
        [MaxLength(100)]  // Adjust based on the longest translated measurement name you expect
        public string TranslatedName { get; set; }
    }

