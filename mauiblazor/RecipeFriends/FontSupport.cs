using NLog;
using SixLabors.Fonts;

namespace RecipeFriends;


public static class FontSupport
{
    private static Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    
	static readonly string[] SupportedFonts = [
		"wwwroot//css//google//fonts//playfair-display-v36-latin_latin-ext-regular.ttf",
		"wwwroot//css//google//fonts//playfair-display-v36-latin_latin-ext-italic.ttf",
		"wwwroot//css//google//fonts//playfair-display-v36-latin_latin-ext-500.ttf",
		"wwwroot//css//google//fonts//playfair-display-v36-latin_latin-ext-500italic.ttf",
		"wwwroot//css//google//fonts//playfair-display-v36-latin_latin-ext-600.ttf",
		"wwwroot//css//google//fonts//playfair-display-v36-latin_latin-ext-600italic.ttf",
		"wwwroot//css//google//fonts//playfair-display-v36-latin_latin-ext-700.ttf",
		"wwwroot//css//google//fonts//playfair-display-v36-latin_latin-ext-700italic.ttf",
		"wwwroot//css//google//fonts//playfair-display-v36-latin_latin-ext-800.ttf",
		"wwwroot//css//google//fonts//playfair-display-v36-latin_latin-ext-800italic.ttf",
		"wwwroot//css//google//fonts//playfair-display-v36-latin_latin-ext-900.ttf",
		"wwwroot//css//google//fonts//playfair-display-v36-latin_latin-ext-900italic.ttf",
		"wwwroot//css//google//fonts//roboto-v30-latin_latin-ext-100.ttf",
		"wwwroot//css//google//fonts//roboto-v30-latin_latin-ext-100italic.ttf",
		"wwwroot//css//google//fonts//roboto-v30-latin_latin-ext-300.ttf",
		"wwwroot//css//google//fonts//roboto-v30-latin_latin-ext-300italic.ttf",				
		"wwwroot//css//google//fonts//roboto-v30-latin_latin-ext-500.ttf",
		"wwwroot//css//google//fonts//roboto-v30-latin_latin-ext-500italic.ttf",								
		"wwwroot//css//google//fonts//roboto-v30-latin_latin-ext-700.ttf",
		"wwwroot//css//microsoft//fonts//georgia.ttf",
		"wwwroot//css//microsoft//fonts//georgiab.ttf",
		"wwwroot//css//microsoft//fonts//georgiai.ttf",
		"wwwroot//css//microsoft//fonts//georgiaz.ttf",
		"Arial.ttf", 
		"Arial Italic.ttf", 
		"Arial Bold.ttf", 
		"Arial Bold Italic.ttf", 				
		"Segoe UI.ttf", 
		"Segoe UI Italic.ttf", 
		"Segoe UI Bold.ttf", 
		"Segoe UI Bold Italic.ttf",
		"OpenSans-Regular.ttf"];
    public static void Setup(){
        SetupFontsFiles(SupportedFonts);
    }    

    public static void SetupFontsFiles(string[] sSupportedFonts)
        {
            List<FontFileInfo> tempFontInfoList = new List<FontFileInfo>();
            foreach (string fontPathFile in sSupportedFonts)
            {
                try
                {

                    FontFileInfo fontInfo = FontFileInfo.LoadAsync(fontPathFile).GetAwaiter().GetResult();

                    tempFontInfoList.Add(fontInfo);
				 	using var fontStream = FileSystem.OpenAppPackageFileAsync(fontPathFile).GetAwaiter().GetResult();
        		    Shared.PDF.ConvertRecipeToPDF.RegisterFont(fontInfo.FontDescription.FontFamilyInvariantCulture, fontStream);


					fontStream.Position = 0;
					using var fontData = SkiaSharp.SKData.Create(fontStream);
					foreach (var index in Enumerable.Range(0, 256))
					{
						var typeface = SkiaSharp.SKTypeface.FromData(fontData, index);
						
						if (typeface == null)
							break;
						
						var s = typeface.IsBold ? "Bold" : (typeface.IsItalic ? "Italic" : "");
						_logger.Info($"{Path.GetFileName(fontPathFile)} - {typeface.FamilyName} - {s} - {typeface.FontWeight} ");

					}

                }
                catch (System.Exception e)
                {
                    _logger.Error(e, "Problem setup of the fonts");
                }
            }
        }
	
        private readonly struct FontFileInfo
        {
            private FontFileInfo(string path, FontDescription fontDescription)
            {
                this.Path = path;
                this.FontDescription = fontDescription;
            }

            public string Path { get; }

            public FontDescription FontDescription { get; }

            public string FamilyName => this.FontDescription.FontFamilyInvariantCulture;

            public static async Task<FontFileInfo> LoadAsync(string path)
            {
#if WINDOWS
            // it seems to crash (wait for ever) on windows. With the synchronous call and GetAwaiter it seems to work...
            // fake the await so that it will keep the compiler happy ;-)
            using var t = FileSystem.OpenAppPackageFileAsync(path).GetAwaiter().GetResult();
            
            //await Task.Delay(1);
#else
            using var t = await FileSystem.OpenAppPackageFileAsync(path);
#endif

            FontDescription fontDescription = FontDescription.LoadDescription(t);
                return new FontFileInfo(path, fontDescription);
            }
        }
}
