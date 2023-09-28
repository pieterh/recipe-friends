
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Document = QuestPDF.Fluent.Document;
using SkiaSharp;

using Markdig;
using Markdig.Syntax;

using RecipeFriends.Shared.DTO;
using RecipeFriends.Shared.PDF.Converters;

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
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(FontSizeBody).FontFamily(FontFamilyBody).FontColor(Colors.Black));

                page.Header()
                    .Column(col => {
                        col.Item().Text(recipeDetails.Title)
                            .FontFamily(FontFamilyH1).FontSize(FontSizeH1).Bold()
                            .FontColor(Colors.Black);
                        col.Item().PaddingVertical(FontSizeBody / 2, Unit.Point);
                        col.Item().LineHorizontal(1, Unit.Point).LineColor("#E8E8E8");
                        col.Item().PaddingVertical(FontSizeBody / 2, Unit.Point);
                    });
                
                page.Content()                    
                    .Column(col => {
                        col.Spacing(FontSizeBody, Unit.Point);
                        col.Item().Text(txt1 => { WriteToPdf(txt1, recipeDetails.ShortDescription); });
                        col.Item().Text(txt2 => { WriteToPdf(txt2, recipeDetails.Description); });

                        col.Item().Column(col => {
                            col.Item().Row(row =>{
                                row.AutoItem().BorderRight(1, Unit.Point).BorderColor("#E8E8E8").PaddingRight(10, Unit.Point).Column(col => {
                                    col.Item().Text("Prep Time").FontColor("#7F7F7F").FontSize(FontSizeBody);
                                    col.Item().Text("15 min").FontColor(Colors.Black).FontSize(FontSizeH2);                                    
                                });
                                row.AutoItem().BorderRight(1, Unit.Point).BorderColor("#E8E8E8").PaddingLeft(10, Unit.Point).PaddingRight(10, Unit.Point).Column(col => {
                                    col.Item().Text("Cooking Time").FontColor("#7F7F7F").FontSize(FontSizeBody);
                                    col.Item().Text("25 min").FontColor(Colors.Black).FontSize(FontSizeH2);
                                });
                                row.AutoItem().BorderRight(1, Unit.Point).BorderColor("#E8E8E8").PaddingLeft(10, Unit.Point).PaddingRight(10, Unit.Point).Column(col => {
                                    col.Item().Text("Servings").FontColor("#7F7F7F").FontSize(FontSizeBody);
                                    col.Item().Text("4 people").FontColor(Colors.Black).FontSize(FontSizeH2);
                                });
                            });
                        });

                        col.Item().Element(x => debugOn ? x.DebugArea() : x)
                            .Row(row => {                            
                                row.RelativeItem(10)
                                        .PaddingRight(10, Unit.Point)
                                        .Column(x => {                                                                                    
                                            WriteDirections(recipeDetails, x);
                                        });
                                        
                                row.RelativeItem(6).Element(x => debugOn ? x.DebugArea() : x).MinimalBox()
                                        .Column(x =>
                                        {
                                            x.Item()
                                                .Component(new InfoBoxList<IngredientItem>("Ingredients", ingredients));

                                            x.Item()
                                                .Element(x => debugOn ? x.DebugArea() : x)
                                                .PaddingTop(10, Unit.Point)
                                                .Component(new InfoBoxList<TextItem>("Equipment", equipment));
                                        });
                                                
                            });
                    });
 
                if (pageNumbers){
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                }
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

    private void WriteDirections(RecipeDetails recipeDetails, ColumnDescriptor x)
    {
        bool debugOn = false;
        x.Spacing(0, Unit.Point);
        x.Item()
            .PaddingBottom(-10, Unit.Point)             // unclear why I need to fix it like this
            .Text(txt => { 
                txt.Line("Directions")
                   .FontSize(FontSizeH2)
                   .Bold(); 
            });

        var i = 1;
        foreach (var l in recipeDetails.Directions.Split('\n'))
        {
            x.Item()
                .ShowEntire()
                .PaddingBottom(10, Unit.Point)
                .Element(x => debugOn ? x.DebugArea() : x)
                .Row(row =>
                {
                    // capture the number to a local variable inside the loop, so that the lambda expression
                    // will get the correct value 
                    int nr = i;
 
                    row.Spacing(5);

                    row.ConstantItem(25, Unit.Point).Element(x => debugOn ? x.DebugArea() : x)
                        .Height(25).Width(25)
                        .Canvas((canvas, size) =>
                        {
                            using var circlePaint = new SKPaint
                            {
                                Color = SKColor.Parse("#FF642F"),
                                Style = SKPaintStyle.Fill
                            };

                            using var textPaint = new SKPaint
                            {
                                Color = SKColors.White,
                                Style = SKPaintStyle.Fill,
                                TextAlign = SKTextAlign.Center,
                                TextSize = FontSizeL1,
                                Typeface = SKTypeface.FromFamilyName(
                                                    FontFamilyLI,
                                                    SKFontStyleWeight.Bold,
                                                    SKFontStyleWidth.Normal,
                                                    SKFontStyleSlant.Italic)
                            };
                            // move origin to the center of the available space
                            canvas.Translate(size.Width / 2, (size.Height / 2) - 3);

                            textPaint.GetFontMetrics(out SKFontMetrics f);
                            float textHeight = f.Descent - f.Ascent;
                            float textOffset = (textHeight / 2) - f.Descent - 1;
                            // draw a circle
                            canvas.DrawCircle(0, 0, 10, circlePaint);
                            canvas.DrawText(nr.ToString(), 0, textOffset, textPaint);
                        });

                    row.RelativeItem().Element(x => debugOn ? x.DebugArea() : x)
                        .PaddingTop(2, Unit.Point)
                        .Text(txt3 =>
                        {
                            txt3.DefaultTextStyle(x => x.FontSize(ConvertRecipeToPDF.FontSizeBody).FontFamily(ConvertRecipeToPDF.FontFamilyBody));
                            WriteToPdf(txt3, l);                        
                        });
                });
            i++;
        }
    }

    private void WriteToPdf(TextDescriptor td, string markdownText){
        MarkdownDocument document = Markdown.Parse(markdownText);
        // Iterate through all MarkdownObjects in a depth-first order
        var root = document as ContainerBlock;
        var rootConverter = new ContainerBlockConverter(root);
        rootConverter.WriteTo(td);      
    }
}

internal class InfoBoxList<T>(string title, IEnumerable<object>  items)  : IComponent where T : ItemDrawer, new()
{
    public string Title { get; } = title;
    public IEnumerable<object>  Items { get; } = items;

    private const string BackgroundColor  = "#F9F9F9";//F9F9F9

    public void Compose(IContainer container)
    {
        bool debugOn = false;
        container.Decoration(decoration => { 
            decoration.Before(before => {
                before             
                    .ShowOnce()
                    .ExtendHorizontal()
                    .Background(BackgroundColor)
                    .Column(col => {                                                        
                        col.Item().Element(x => debugOn ? x.DebugArea() : x)
                            .PaddingTop(10, Unit.Point)
                            .PaddingLeft(10, Unit.Point)
                            .PaddingRight(10, Unit.Point)
                            .PaddingBottom(-10, Unit.Point) // not clear why I need to fix it like this
                            .Text(txt =>{
                                txt.DefaultTextStyle(x => x.FontSize(ConvertRecipeToPDF.FontSizeH2).FontFamily(ConvertRecipeToPDF.FontFamilyH2));
                                txt.Line(Title).Bold();
                        });                                                        
                    });
            });
            decoration.Content(content => {                                                
                content
                    .ShowOnce()
                    .ExtendHorizontal()
                    .Background(BackgroundColor)  
                    .PaddingBottom(10, Unit.Point)
                    .Element(x => debugOn ? x.DebugArea() : x)                                   
                    .Column(col => {                                              
                        foreach (var (item, index) in Items.Select((value, idx) => (value, idx)))
                        {
                            var t = new T
                            {
                                Item = item,
                                Index = index
                            };
                            col.Item()                            
                                .PaddingLeft(10, Unit.Point)
                                .PaddingRight(10, Unit.Point)
                                .DefaultTextStyle(x => x.FontSize(ConvertRecipeToPDF.FontSizeBody).FontFamily(ConvertRecipeToPDF.FontFamilyBody))
                                .Component(t);
                        }
                    });
            });
        });
    }
}

internal abstract class ItemDrawer : IComponent {
    
    internal ItemDrawer() { }

    public object? Item {get; set;}
    public int Index {get; set;}

    public abstract void Compose(IContainer container);
}


internal class TextItem : ItemDrawer
{
    public override void Compose(IContainer container)
    {
        if (Item == null) throw new InvalidOperationException("The property Item should not be null");
        container.Row(row =>
                    {
                        row.Spacing(5);
                        row.AutoItem().Text($"{Index + 1}.");
                        row.RelativeItem().Text(Item.ToString());
                    });
    }
}

internal class IngredientItem : ItemDrawer
{
    private const string BorderColor = "#D3D3D3";//E8E8E8
    public override void Compose(IContainer container)
    {
        if (Item == null) throw new InvalidOperationException("The property Item should not be null");
        var ingredient = Item as IngredientDetails;
        if (ingredient == null) throw new InvalidOperationException("The property Item should be of type IngredientDetails");
        container.BorderColor(BorderColor).BorderBottom(0.5f, Unit.Point).Inlined(inlined => {    
                    inlined.Item().Row(row =>
                    {    
                        row.Spacing(5);
                        row.AutoItem().Text($"{Index + 1}.");
                        row.RelativeItem().AlignLeft().Text($"{ingredient.Name}" );
                        row.RelativeItem().AlignRight().Text($"{ingredient.Amount} {ingredient.Measurement}" );
                    });
        });
    }
}
