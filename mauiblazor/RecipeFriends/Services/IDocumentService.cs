namespace RecipeFriends;

public interface IDocumentService
{
        Task<byte[]> RecipeToPDFAsync(int id, CancellationToken cancellationToken);
}
