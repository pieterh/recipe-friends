namespace RecipeFriends;

public interface IDocumentService
{
        Task<string> RecipeToMarkdownAsync(int id, CancellationToken cancellationToken);
        Task<byte[]> RecipeToPDFAsync(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Creates for each page in the pdf a png image
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<byte[]>> RecipeToImageAsync(int id, CancellationToken cancellationToken);
}
