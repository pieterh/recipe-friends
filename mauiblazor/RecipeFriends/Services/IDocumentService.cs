namespace RecipeFriends;

public interface IDocumentService
{
        Task<string> RecipeToHtmlAsync(int id, CancellationToken cancellationToken);
        Task<byte[]> RecipeToPDFAsync(int id, CancellationToken cancellationToken);
}
