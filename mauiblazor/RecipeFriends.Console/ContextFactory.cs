using Microsoft.EntityFrameworkCore.Design;
namespace RecipeFriends.Console;
using RecipeFriends.Data;

public class ContextFactory : IDesignTimeDbContextFactory<RecipeFriendsContext>
{
    public RecipeFriendsContext CreateDbContext(string[] args) {
        return new RecipeFriendsContext();
    }       
}

