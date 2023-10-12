using System.ComponentModel.DataAnnotations;

namespace RecipeFriends.Shared.DTO;
public class CategoryInfo
{
    public static CategoryInfo Unset { get { return new CategoryInfo() { Id = -1, Name = string.Empty }; } }

    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public required string Name { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }
        var other = (CategoryInfo)obj;
        return other?.Id == Id;
    }
    public override int GetHashCode() => Id.GetHashCode();
    public override string ToString()
    {
        return Name;
    }
}
