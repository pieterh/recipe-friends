namespace RecipeFriends.Services;

/// <summary>
/// Exception occurred if file is not saved
/// </summary>
public sealed class FolderPickerException : Exception
{
	/// <summary>
	/// Initializes a new instance of <see cref="FolderPickerException"/>
	/// </summary>
	public FolderPickerException(string message) : base(message)
	{

	}
}