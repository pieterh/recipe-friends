
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace RecipeFriends.Shared.PDF;

public class PageConfiguration
{
    public static void ApplyStandardPageConfiguration(PageDescriptor page, string title, bool pageNumbers)
    {
        page.Size(PageSizes.A4);
        page.Margin(1.5f, Unit.Centimetre);
        page.PageColor(Colors.White);
        page.DefaultTextStyle(x => x.FontSize(ConvertRecipeToPDF.FontSizeBody)
                                     .FontFamily(ConvertRecipeToPDF.FontFamilyBody)
                                     .FontColor(Colors.Black));

        page.Header()
            .Column(col => {
                col.Item().Text(title)
                    .FontFamily(ConvertRecipeToPDF.FontFamilyH1).FontSize(ConvertRecipeToPDF.FontSizeH1).Bold()
                    .FontColor(Colors.Black);
                col.Item().PaddingVertical(ConvertRecipeToPDF.FontSizeBody / 2, Unit.Point);
                col.Item().LineHorizontal(1, Unit.Point).LineColor("#E8E8E8");
                col.Item().PaddingVertical(ConvertRecipeToPDF.FontSizeBody / 2, Unit.Point);
                    });

        if (pageNumbers)
        {
            page.Footer()
                .AlignCenter()
                .Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                });
        }
    }
}
