namespace RecipeFriends.Shared.DTO;

public class ImageData: ImageInfo
{
    private byte[] _data;
    private string _dataHash = string.Empty;

    public byte[] Data
    {
        get => _data;
        set
        {
            if (_data != value)
            {
                _data = value;
                if (value != null && value.Length > 50){
                    _dataHash = GetHash(value);
                }else{
                    _dataHash = string.Empty;
                }
                InvalidateHash();
            }
        }
    }

    protected override void CalculateHash()
    {
        // Combine the values of Id, Name to calculate the hash value
        if (string.IsNullOrEmpty(_dataHash)){
            string combinedValues = $"{Id}{Name}{_dataHash}";
            HashValue = GetHash(combinedValues);
        }else   
            HashValue = _dataHash;
    }

     protected string GetHash( byte[] data)
    {
        using (var hasher = System.Security.Cryptography.MD5.Create())
        {
            byte[] hashBytes = hasher.ComputeHash(data);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }
}
