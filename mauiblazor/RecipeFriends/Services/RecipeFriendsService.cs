namespace RecipeFriends.Services;
public class RecipeFriendsService : IRecipeFriendsService
{
    public static readonly string DocumentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Recipe Friends");
    public string GetDocumentsPath() => DocumentsPath; 
}
