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
    [StringLength(100, MinimumLength = 25)]
    public required string ShortDescription { get; set; }

    [Required]
    [StringLength(4096, MinimumLength = 1)]
    public required string Description { get; set; }

    [Required]
    [StringLength(4096, MinimumLength = 25)]
    public required string Directions { get; set; }

    [Required]
    public required TimeOnly PreparationTime { get; set; }

    [Required]
    public required TimeOnly CookingTime { get; set; }

    public virtual ICollection<IngredientDetails> Ingredients { get; set; } = new List<IngredientDetails>();

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

public enum Measurement
{
    Tablespoon = 10,
    Teaspoon = 20,
    Milliliter = 30,
    Deciliter = 40,
    Liter = 50,
    Milligram = 60,
    Gram = 70
}
