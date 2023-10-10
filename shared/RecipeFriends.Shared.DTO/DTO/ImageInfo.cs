using System;
using System.Collections.Generic;
using System.Linq;

public class ImageInfo
{
    private int id;
    private int order;
    private string title;
    private string name;
    private string? hashValue;

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
                InvalidateHash();
            }
        }
    }

    public string Title
    {
        get => title;
        set
        {
            if (title != value)
            {
                title = value;
                InvalidateHash();
            }
        }
    }

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
    }

    private void InvalidateHash()
    {
        hashValue = null;
    }

    private void CalculateHash()
    {
        // Combine the values of Id, Order, Title, and Name to calculate the hash value
        string combinedValues = $"{Id}{Order}{Title}{Name}";
        hashValue = GetHash(combinedValues);
    }

    private string GetHash(string input)
    {
        using (var hasher = System.Security.Cryptography.MD5.Create())
        {
            byte[] data = hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(data).Replace("-", "").ToLowerInvariant();
        }
    }
}
