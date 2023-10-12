namespace RecipeFriends.Shared.DTO;

public class ImageData: ImageInfo
{
    private byte[] _data = [];
    private string _dataHash = string.Empty;

    // Only the data DTO version has the status. It is used to keep 
    // track of new and removed items before saving updates
    public ImageStatus Status { get; set; }

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

     protected static string GetHash( byte[] data)
    {
        byte[] hashBytes = System.Security.Cryptography.MD5.HashData(data);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}
