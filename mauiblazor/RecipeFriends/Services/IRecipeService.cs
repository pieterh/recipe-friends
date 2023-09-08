using RecipeFriends.Shared.DTO.v1;

namespace RecipeFriends.Services;

public interface IRecipeService
{
    Task<RecipeInfo[]> GetRecipesAsync(CancellationToken cancellationToken);
    Task<RecipeDetails> GetRecipeDetailsAsync(int id, CancellationToken cancellationToken);
    Task<bool> SaveRecipeDetailsAsync(RecipeDetails recipeDetails, CancellationToken cancellationToken);
}