

using Markdig.Syntax;
using QuestPDF.Fluent;

namespace RecipeFriends.Shared.PDF.Converters;

public class ContainerBlockConverter{

    private ContainerBlock containerBlock;

    public ContainerBlockConverter(ContainerBlock cb){
        containerBlock = cb;
    }

    internal void WriteTo(TextDescriptor text)
    {
        foreach(var item in containerBlock){
            if (item is ParagraphBlock paragraphBlock){
                var paragraph = new ParagraphBlockConverter(paragraphBlock);
                paragraph.WriteTo(text);
            }
        }
    }
}