using System.ComponentModel.DataAnnotations;

namespace RecipeFriends.Shared.DTO;

public class IngredientDetails
{
    [Range(1, Double.MaxValue)]
    public int Id { get; set; }

    [StringLength(50, MinimumLength = 5)]
    public required string Name { get; set; }

    [Range(1, 1000)]
    public required double Amount { get; set; }

    public required MeasurementInfo Measurement { get; set; }

    public required int Order { get; set; }
}