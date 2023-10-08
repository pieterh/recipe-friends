﻿
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Document = QuestPDF.Fluent.Document;

using RecipeFriends.Shared.DTO;
using RecipeFriends.Shared.PDF.Components;

namespace RecipeFriends.Shared.PDF;

public class ConvertRecipeToPDF
{
    internal const float FontSizeH1 = 20;
    internal const float FontSizeH2 = 16;
    internal const float FontSizeBody = 10;
    internal const float FontSizeL1 = 10;

    internal const string FontFamilyH1 = "Playfair Display";
    internal const string FontFamilyH2 = "Georgia";
    internal const string FontFamilyBody = "Georgia";

    internal const string FontFamilyLI = "Georgia";

    static ConvertRecipeToPDF(){
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public static void RegisterFont(Stream s) {
        FontManager.RegisterFont(s);
    }

    public static void RegisterFont(string fontName, Stream s) {
        FontManager.RegisterFontWithCustomName(fontName, s);
    }

    public IEnumerable<byte[]> ToImage(RecipeDetails recipeDetails, bool pageNumbers = false)
    {
        var document = ToDocument(recipeDetails, pageNumbers);        
        var data = document.GenerateImages(new ImageGenerationSettings(){ ImageFormat = ImageFormat.Png, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 300});
        return data;
    }

    public byte[] DoTest(RecipeDetails recipeDetails, bool pageNumbers = false)
    {
        var document = ToDocument(recipeDetails, pageNumbers);        
        var data = document.GeneratePdf();
        return data;
    }
        
    private Document ToDocument(RecipeDetails recipeDetails, bool pageNumbers)
    {
        var ingredients = recipeDetails.Ingredients;
        IEnumerable<string> equipment = ["Skillet", "Rookplankje"];

        bool debugOn = false;
        var document = QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                PageConfiguration.ApplyStandardPageConfiguration(page, recipeDetails.Title, pageNumbers);
               
                page.Content()                    
                    .Column(col => {
                        col.Spacing(FontSizeBody, Unit.Point);
                        col.Item().Text(txt1 => { MarkdownToPDF.WriteToPdf(txt1, recipeDetails.ShortDescription); });
                        col.Item().Text(txt2 => { MarkdownToPDF.WriteToPdf(txt2, recipeDetails.Description); });

                        col.Item().Column(col =>
                        {
                            AddRecipeQuickStats(col.Item());
                        });

                        col.Item().Element(x => debugOn ? x.DebugArea() : x)
                            .Row(row => {                            
                                row.RelativeItem(10)
                                        .PaddingRight(10, Unit.Point)
                                        .Column(x => {
                                            RecipeDirections.WriteDirections(recipeDetails, x);
                                        });
                                        
                                row.RelativeItem(6).Element(x => debugOn ? x.DebugArea() : x).MinimalBox()
                                        .Column(x =>
                                        {
                                            x.Item()
                                                .Component(new InfoBoxList<IngredientItemDrawer>("Ingredients", ingredients));

                                            x.Item()
                                                .Element(x => debugOn ? x.DebugArea() : x)
                                                .PaddingTop(10, Unit.Point)
                                                .Component(new InfoBoxList<TextItemDrawer>("Equipment", equipment));
                                        });
                                                
                            });
                    });
            });
        });
        var metadata = new DocumentMetadata
        {
            Title = recipeDetails.Title,
            Keywords = recipeDetails.TagsAsString
        };
        document.WithMetadata(metadata);
        return document;
    }

    private static void AddRecipeQuickStats(IContainer container)
    {        
        container.Row(row =>
        {
            row.AutoItem().BorderRight(1, Unit.Point).BorderColor("#E8E8E8").PaddingRight(10, Unit.Point).Column(col =>
            {
                col.Item().Text("Prep Time").FontColor("#7F7F7F").FontSize(FontSizeBody);
                col.Item().Text("15 min").FontColor(Colors.Black).FontSize(FontSizeH2);
            });
            row.AutoItem().BorderRight(1, Unit.Point).BorderColor("#E8E8E8").PaddingLeft(10, Unit.Point).PaddingRight(10, Unit.Point).Column(col =>
            {
                col.Item().Text("Cooking Time").FontColor("#7F7F7F").FontSize(FontSizeBody);
                col.Item().Text("25 min").FontColor(Colors.Black).FontSize(FontSizeH2);
            });
            row.AutoItem().BorderRight(1, Unit.Point).BorderColor("#E8E8E8").PaddingLeft(10, Unit.Point).PaddingRight(10, Unit.Point).Column(col =>
            {
                col.Item().Text("Servings").FontColor("#7F7F7F").FontSize(FontSizeBody);
                col.Item().Text("4 people").FontColor(Colors.Black).FontSize(FontSizeH2);
            });
        });
    }
}
