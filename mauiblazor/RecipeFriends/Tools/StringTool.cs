namespace RecipeFriends;

public class StringTool
{
    public static string SizeToReadableString(long size)
    {
        if (size < 1024)
        {
            // Size is less than 1KB
            return $"{size} bytes";
        }
        else if (size < 1024 * 1024)
        {
            // Size is between 1KB and 1MB
            double fileSizeKB = Math.Round(size / 1024.0, 2);
            return $"{fileSizeKB} KB";
        }
        else if (size < 1024 * 1024 * 1024)
        {
            // Size is between 1MB and 1GB
            double fileSizeMB = Math.Round(size / (1024.0 * 1024.0), 2);
            return $"{fileSizeMB} MB";
        }
        else
        {
            // Size is greater than or equal to 1GB
            double fileSizeGB = Math.Round(size / (1024.0 * 1024.0 * 1024.0), 2);
            return $"{fileSizeGB} GB";
        }
    }   
}

public static class StringExtensions
{
    public static string ToReadableSize(this long size)
    {
        return StringTool.SizeToReadableString(size);
    }
}

