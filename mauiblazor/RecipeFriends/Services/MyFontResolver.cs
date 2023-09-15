
using System.Diagnostics.CodeAnalysis;
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Internal;
using PdfSharpCore.Utils;
using SixLabors.Fonts;

internal class MyFontResolver : IFontResolver
{
        public string DefaultFontName => "Arial";

        private static readonly Dictionary<string, FontFamilyModel> InstalledFonts = new Dictionary<string, FontFamilyModel>();

        private static readonly string[] SSupportedFonts;

        static MyFontResolver()
        {
            Console.WriteLine(Microsoft.Maui.Devices.DeviceInfo.Platform);
            Console.WriteLine(FileSystem.AppDataDirectory);
            Console.WriteLine(FileSystem.CacheDirectory);            

            SSupportedFonts = new string[] {
                "Arial.ttf", "Arial Italic.ttf", "Arial Bold.ttf", "Arial Bold Italic.ttf", 
                "Segoe UI.ttf", "Segoe UI Italic.ttf", "Segoe UI Bold.ttf", "Segoe UI Bold Italic.ttf",
                "OpenSans-Regular.ttf"};
            SetupFontsFiles(SSupportedFonts);
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


            public XFontStyle GuessFontStyle()
            {
                switch (this.FontDescription.Style)
                {
                    case FontStyle.Bold:
                        return XFontStyle.Bold;
                    case FontStyle.Italic:
                        return XFontStyle.Italic;
                    case FontStyle.BoldItalic:
                        return XFontStyle.BoldItalic;
                    default:
                        return XFontStyle.Regular;
                }
            }

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
        public static void SetupFontsFiles(string[] sSupportedFonts)
        {
            List<FontFileInfo> tempFontInfoList = new List<FontFileInfo>();
            foreach (string fontPathFile in sSupportedFonts)
            {
                try
                {
                    FontFileInfo fontInfo = FontFileInfo.LoadAsync(fontPathFile).GetAwaiter().GetResult();
                    Console.WriteLine(fontPathFile);
                    tempFontInfoList.Add(fontInfo);
                }
                catch (System.Exception e)
                {
#if DEBUG
                    System.Console.Error.WriteLine(e);
#endif
                }
            }

            // Deserialize all font families
            foreach (IGrouping<string, FontFileInfo> familyGroup in tempFontInfoList.GroupBy(info => info.FamilyName))
                try
                {
                    string familyName = familyGroup.Key;
                    FontFamilyModel family = DeserializeFontFamily(familyName, familyGroup);
                    InstalledFonts.Add(familyName.ToLower(), family);
                }
                catch (System.Exception e)
                {
#if DEBUG
                    System.Console.Error.WriteLine(e);
#endif
                }
        }


        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private static FontFamilyModel DeserializeFontFamily(string fontFamilyName, IEnumerable<FontFileInfo> fontList)
        {
            FontFamilyModel font = new FontFamilyModel { Name = fontFamilyName };

            // there is only one font
            if (fontList.Count() == 1)
                font.FontFiles.Add(XFontStyle.Regular, fontList.First().Path);
            else
            {
                foreach (FontFileInfo info in fontList)
                {
                    XFontStyle style = info.GuessFontStyle();
                    if (!font.FontFiles.ContainsKey(style))
                        font.FontFiles.Add(style, info.Path);
                }
            }

            return font;
        }

        public virtual byte[] GetFont(string faceFileName)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                string ttfPathFile = "";
                try
                {
                    ttfPathFile = SSupportedFonts.ToList().First(x => x.ToLower().Contains(
                        System.IO.Path.GetFileName(faceFileName).ToLower())
                    );

                    using (var ttf = FileSystem.OpenAppPackageFileAsync(ttfPathFile).Result)
                    //using (System.IO.Stream ttf = System.IO.File.OpenRead(ttfPathFile))
                    {
                        ttf.CopyTo(ms);
                        ms.Position = 0;
                        return ms.ToArray();
                    }
                }
                catch (System.Exception e)
                {
                    System.Console.WriteLine(e);
                    throw new System.Exception("No Font File Found - " + faceFileName + " - " + ttfPathFile);
                }
            }
        }

        public bool NullIfFontNotFound { get; set; } = false;

        public virtual FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (InstalledFonts.Count == 0)
                throw new System.IO.FileNotFoundException("No Fonts installed on this device!");

            if (InstalledFonts.TryGetValue(familyName.ToLower(), out FontFamilyModel family))
            {
                if (isBold && isItalic)
                {
                    if (family.FontFiles.TryGetValue(XFontStyle.BoldItalic, out string boldItalicFile))
                        return new FontResolverInfo(System.IO.Path.GetFileName(boldItalicFile));
                }
                else if (isBold)
                {
                    if (family.FontFiles.TryGetValue(XFontStyle.Bold, out string boldFile))
                        return new FontResolverInfo(System.IO.Path.GetFileName(boldFile));
                }
                else if (isItalic)
                {
                    if (family.FontFiles.TryGetValue(XFontStyle.Italic, out string italicFile))
                        return new FontResolverInfo(System.IO.Path.GetFileName(italicFile));
                }

                if (family.FontFiles.TryGetValue(XFontStyle.Regular, out string regularFile))
                    return new FontResolverInfo(System.IO.Path.GetFileName(regularFile));

                return new FontResolverInfo(System.IO.Path.GetFileName(family.FontFiles.First().Value));
            }

            if (NullIfFontNotFound)
                return null;

            string ttfFile = InstalledFonts.First().Value.FontFiles.First().Value;
            return new FontResolverInfo(System.IO.Path.GetFileName(ttfFile));
        }
}