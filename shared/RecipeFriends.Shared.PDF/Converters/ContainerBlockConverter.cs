

using Markdig.Syntax;
using QuestPDF.Fluent;

namespace RecipeFriends.Shared.PDF.Converters;

public class ContainerBlockConverter
{
    private ContainerBlock containerBlock;

    public ContainerBlockConverter(ContainerBlock cb)
    {
        containerBlock = cb;
    }

    internal void WriteTo(TextDescriptor text)
    {
        foreach (var item in containerBlock)
        {
            switch (item)
            {
                case ListBlock list:
                    var listConverter = new ListBlockConverter(list);
                    listConverter.WriteTo(text);
                    break;

                case ContainerBlock container:
                    var containerConverter = new ContainerBlockConverter(container);
                    containerConverter.WriteTo(text);
                    break;

                case ParagraphBlock paragraph:
                    var paragraphConverter = new ParagraphBlockConverter(paragraph);
                    paragraphConverter.WriteTo(text);
                    break;
            }
        }
    }
}