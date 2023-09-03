using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RecipeFriends.Shared.DTO.v1;

public class RecipeDetails
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

    [Required]
    [StringLength(4096, MinimumLength = 100)]
    public required string Description { get; set; }

    [Required]
    [StringLength(4096, MinimumLength = 100)]
    public required string Directions { get; set; }

    [Required]        
    public required TimeOnly PreparationTime { get; set; }

    [Required]
    public required TimeOnly CookingTime { get; set; }

    public List<string> Tags { get; set; } = new List<string>();
}

public enum Catagories
{
    AmuseBouche = 10,
    Appetiser = 20,
    Soup = 30,
    Salad = 40,
    MainCourse = 50,
    Dessert = 60,
    Sauce = 70,
    Drink = 80
}
