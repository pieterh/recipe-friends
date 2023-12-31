﻿using System.ComponentModel.DataAnnotations;

namespace RecipeFriends.Shared.DTO;

public class RecipeDetails
{
    [Range(-1, Double.MaxValue)]
    public int Id { get; set; }

    [Required]
    [StringLength(64, MinimumLength = 10)]
    public required string Title { get; set; }

    [Required]
    public CategoryInfo Category { get; set; } = CategoryInfo.Unset;

    [Required]
    [StringLength(250, MinimumLength = 25)]
    public required string ShortDescription { get; set; }

    [Required]
    [StringLength(4096, MinimumLength = 25)]
    public required string Description { get; set; }

    [Required]
    [StringLength(4096, MinimumLength = 25)]
    public required string Directions { get; set; }

    [StringLength(4096, MinimumLength = 0)]
    public string? Notes { get; set; }

    [Required]
    public required TimeOnly PreparationTime { get; set; }

    [Required]
    public required TimeOnly CookingTime { get; set; }

    public virtual ICollection<IngredientDetails> Ingredients { get; set; } = new List<IngredientDetails>();

    public List<TagInfo> Tags { get; set; } = [];
    public List<EquipmentInfo> Equipment { get; set; } = [];
    public string TagsAsString { get { return string.Join(", ", Tags.Order().Select((t) => t.Name)); } }

    public List<ImageInfo> Images { get; set; } = [];


}

