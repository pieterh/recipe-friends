using RecipeFriends.Shared.DTO;

namespace RecipeFriends.Services;

public interface IRecipeService
{
    Task<RecipeInfo[]> GetRecipesAsync(CancellationToken cancellationToken);
    Task<RecipeDetails> GetRecipeDetailsAsync(int id, CancellationToken cancellationToken);
    Task<bool> SaveRecipeDetailsAsync(RecipeDetails recipeDetails, CancellationToken cancellationToken);

    Task<TagInfo[]> GetTagsAsync(CancellationToken cancellationToken);

    Task<CategoryInfo[]> GetCategoriesAsync(CancellationToken cancellationToken);

    Task<MeasurementInfo[]> GetMeasurementsAsync(CancellationToken cancellationToken);
}