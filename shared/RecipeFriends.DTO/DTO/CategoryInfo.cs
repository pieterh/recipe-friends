namespace RecipeFriends.Shared.DTO;
public class CategoryInfo
{
    public int Id { get; set; }
    public string Name { get; set; }

    public override bool Equals(object o) {
        var other = o as CategoryInfo;
        return other?.Id == Id;
    }
    public override int GetHashCode() => Id.GetHashCode();        
    public override string ToString() {
        return Name;
    }
}
