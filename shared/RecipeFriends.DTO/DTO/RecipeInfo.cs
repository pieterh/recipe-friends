using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RecipeFriends.Shared.DTO;

public class RecipeInfo
{
    [Range(1, Double.MaxValue)]
    public int Id { get; set; }

    [Required]
    [StringLength(64, MinimumLength = 10)]
    public required string Title { get; set; }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required Catagories Catagory { get; set; }        

    [Required]
    [StringLength(100, MinimumLength = 50)]
    public required string ShortDescription { get; set; }

    public List<string> Tags { get; set; } = new List<string>();
}
