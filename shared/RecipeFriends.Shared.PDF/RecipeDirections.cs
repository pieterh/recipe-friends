using Markdig;
using Markdig.Syntax;
using SkiaSharp;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

using RecipeFriends.Shared.DTO;
using RecipeFriends.Shared.PDF.Converters;

namespace RecipeFriends.Shared.PDF;
public class RecipeDirections
{

    public static void WriteDirections(RecipeDetails recipeDetails, ColumnDescriptor x)
    {
        bool debugOn = false;
        x.Spacing(0, Unit.Point);
        x.Item()
            .PaddingBottom(-10, Unit.Point)             // unclear why I need to fix it like this
            .Text(txt =>
            {
                txt.Line("Directions")
                   .FontSize(ConvertRecipeToPDF.FontSizeH2)
                   .Bold();
            });

        var i = 1;
        foreach (var l in recipeDetails.Directions.Split('\n'))
        {
            // strip empty rows from the list of directions
            if (!string.IsNullOrEmpty(l))
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
                                    TextSize = ConvertRecipeToPDF.FontSizeL1,
                                    Typeface = SKTypeface.FromFamilyName(
                                                        ConvertRecipeToPDF.FontFamilyLI,
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
                                MarkdownToPDF.WriteToPdf(txt3, l);
                            });
                    });
                i++;
            }
        }
    }
}