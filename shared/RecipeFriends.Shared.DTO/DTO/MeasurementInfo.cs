namespace RecipeFriends.Shared.DTO;
public class MeasurementInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public override bool Equals(object o)
    {
        var other = o as MeasurementInfo;
        return other?.Id == Id;
    }
    public override int GetHashCode() => Id.GetHashCode();
    public override string ToString()
    {
        return Name;
    }
}