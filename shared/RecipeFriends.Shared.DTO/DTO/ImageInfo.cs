using System.ComponentModel.DataAnnotations;

namespace RecipeFriends.Shared.DTO;

public class ImageInfo
{
    private int id;
    private int order;

    private string title = string.Empty;

    private string name = string.Empty;
    private string hashValue = string.Empty;

    public int Id
    {
        get => id;
        set
        {
            if (id != value)
            {
                id = value;
                InvalidateHash();
            }
        }
    }

    public int Order
    {
        get => order;
        set
        {
            if (order != value)
            {
                order = value;
            }
        }
    }

    [Required]
    [StringLength(100, MinimumLength = 10)]
    public string Title
    {
        get => title;
        set
        {
            if (title != value)
            {
                
                title = value;
            }
        }
    }

    [Required]
    [StringLength(100, MinimumLength = 10)]
    public string Name
    {
        get => name;
        set
        {
            if (name != value)
            {
                name = value;
                InvalidateHash();
            }
        }
    }

    public string HashValue
    {
        get
        {
            if (string.IsNullOrEmpty(hashValue))
            {
                CalculateHash();
            }
            return hashValue;
        }
        protected set
        {
            hashValue = value;
        }
    }

    protected void InvalidateHash()
    {
        HashValue = string.Empty;
    }

    virtual protected void CalculateHash()
    {
        // Combine the values of Id, Order, Title, and Name to calculate the hash value
        string combinedValues = $"{Id}{Name}";
        HashValue = GetHash(combinedValues);
    }

    protected static string GetHash(string input)
    {
        byte[] data = System.Security.Cryptography.MD5.HashData(System.Text.Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(data).Replace("-", "").ToLowerInvariant();
    }
}
