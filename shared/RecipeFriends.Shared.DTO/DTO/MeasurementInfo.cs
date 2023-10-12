using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RecipeFriends.Shared.DTO;
public class MeasurementInfo
{
    // Parameterless constructor for EF
    protected MeasurementInfo()
    {
        Name = string.Empty;
    }

    // Constructor for application use
    public MeasurementInfo(int id, string name)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public static MeasurementInfo Unset { get { return new MeasurementInfo() { Id = -1, Name = string.Empty }; } }

    [Range(1, Double.MaxValue)]
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (MeasurementInfo)obj;
        return other.Id == Id;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public override string ToString()
    {
        return Name;
    }
}