using Markdig;
using Markdig.Syntax;
using QuestPDF.Fluent;
using RecipeFriends.Shared.PDF.Converters;

namespace RecipeFriends.Shared.PDF;
public class MarkdownToPDF
{
    public static void WriteToPdf(TextDescriptor td, string markdownText)
    {
        MarkdownDocument document = Markdown.Parse(markdownText);
        // Iterate through all MarkdownObjects in a depth-first order
        var root = document as ContainerBlock;
        var rootConverter = new ContainerBlockConverter(root);
        rootConverter.WriteTo(td);
    }
}