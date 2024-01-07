using Markdig.Syntax;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace RecipeFriends.Shared.PDF.Converters;

public class ListBlockConverter
{
    private ListBlock listBlock;

    public ListBlockConverter(ListBlock lb)
    {
        listBlock = lb;
    }

    internal void WriteTo(TextDescriptor text)
    {
        bool debugOn = false;
        int i = 0;
        foreach (var item in listBlock)
        {            
            text.Element().Column(c => { 
                c.Item()
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
                                    Color = SKColors.Black, // SKColor.Parse("#FF642F"),
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
                                canvas.DrawCircle(0, 0, 3, circlePaint);
                                //canvas.DrawText(nr.ToString(), 0, textOffset, textPaint);
                            });

                        row.RelativeItem().Element(x => debugOn ? x.DebugArea() : x)
                            .PaddingTop(2, Unit.Point)
                            .Text(txt3 =>
                            {
                                //txt3.DefaultTextStyle(x => x.FontSize(ConvertRecipeToPDF.FontSizeBody).FontFamily(ConvertRecipeToPDF.FontFamilyBody));
                                //MarkdownToPDF.WriteToPdf(txt3, l.Replace("* ", string.Empty));
                                switch (item)
                                {
                                    case ContainerBlock container:
                                        var containerConverter = new ContainerBlockConverter(container);
                                        containerConverter.WriteTo(txt3);
                                        break;

                                    case ParagraphBlock paragraph:
                                        var paragraphConverter = new ParagraphBlockConverter(paragraph);
                                        paragraphConverter.WriteTo(txt3);
                                        break;

                                }
                            });
                    });
            });
        }
    }
}