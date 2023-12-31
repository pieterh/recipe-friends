﻿using RecipeFriends.Shared.DTO;

namespace RecipeFriends.Services;

public interface IRecipeService
{
    Task<RecipeInfo[]> GetRecipesAsync(CancellationToken cancellationToken);
    Task<RecipeDetails> GetRecipeDetailsAsync(int id, CancellationToken cancellationToken);
    Task<ImageData> GetImageDataAsync(int id, CancellationToken cancellationToken);
    Task<ImageData> GetImageDataAsync(int id, int widht,        int height, CancellationToken cancellationToken);
            
    Task<bool> SaveRecipeDetailsAsync(RecipeDetails recipeDetails, IEnumerable<ImageData> images, CancellationToken cancellationToken);
    Task<bool> SaveImageDataAsync(int recipeId, ImageData img, CancellationToken cancellationToken);

    Task<TagInfo[]> GetTagsAsync(CancellationToken cancellationToken);
    Task<EquipmentInfo[]> GetEquipmentAsync(CancellationToken cancellationToken);
    Task<CategoryInfo[]> GetCategoriesAsync(CancellationToken cancellationToken);

    Task<MeasurementInfo[]> GetMeasurementsAsync(CancellationToken cancellationToken);
}