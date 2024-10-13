using System.Runtime.Versioning;

namespace RecipeFriends.Services;

/// <summary>
/// Allows picking folders from the file system
/// </summary>
public interface IFolderPicker
{
	/// <summary>
	/// Allows the user to pick a folder from the file system
	/// </summary>
	/// <param name="initialPath">Initial path</param>
	/// <param name="cancellationToken"><see cref="CancellationToken"/></param>
	/// <returns><see cref="FolderPickerResult"/></returns>
	[SupportedOSPlatform("iOS14.0")]
	[SupportedOSPlatform("MacCatalyst14.0")]

	Task<FolderPickerResult> PickAsync(string initialPath, CancellationToken cancellationToken = default);

	/// <summary>
	/// Allows the user to pick a folder from the file system
	/// </summary>
	/// <param name="cancellationToken"><see cref="CancellationToken"/></param>
	/// <returns><see cref="FolderPickerResult"/></returns>

	[SupportedOSPlatform("iOS14.0")]
	[SupportedOSPlatform("MacCatalyst14.0")]
	Task<FolderPickerResult> PickAsync(CancellationToken cancellationToken = default);
}