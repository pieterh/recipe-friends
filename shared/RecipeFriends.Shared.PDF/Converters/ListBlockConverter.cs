using Markdig.Syntax;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

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

        //  row.ConstantItem(25, Unit.Point).Element(x => debugOn ? x.DebugArea() : x)
        //                     .Height(25).Width(25)
        //                     .Svg(handler => {
        //                             return $"""
        //                                      <svg height="10" width="10" xmlns="http://www.w3.org/2000/svg">
        //                                        <circle r="3" cx="{25 / 2}" cy="{(25 / 2) - 3}" fill="black" />
        //                                       </svg>
        //                                     """;
        //                         });

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