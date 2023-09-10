﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RecipeFriends.Shared.DTO;

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
    [StringLength(250, MinimumLength = 25)]
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

