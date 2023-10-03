using System.Text;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using QuestPDF.Fluent;

namespace RecipeFriends.Shared.PDF.Converters;

enum ElementType { Bold, Italic };

public class ParagraphBlockConverter
{
    private ParagraphBlock paragraphBlock;
    public ParagraphBlockConverter(ParagraphBlock pb)
    {
        paragraphBlock = pb;
    }

    internal void WriteTo(TextDescriptor text)
    {
        // text.DefaultTextStyle(x => x.FontSize(ConvertRecipeToPDF.FontSizeBody));   
        // text.DefaultTextStyle(x => x.FontFamily(ConvertRecipeToPDF.FontFamilyBody));     
        foreach (var item in paragraphBlock.Inline)
        {
            // if (item is LeafInline l){

            // }else
            if (item is LiteralInline literal)
            {
                ParagraphBlockConverter.AddLiteral(text, literal);
            }
            else
            if (item is EmphasisInline emphasis)
            {
                AddEmphasis(text, emphasis);
                var t = new ContainerInline();
            }
        }
    }

    private static void AddLiteral(TextDescriptor td, LiteralInline literalInline)
    {
        var text = literalInline.Content.ToString();
        td.Span(text);
        //          .FontSize(ConvertRecipeToPDF.FontSizeBody)
        //          .FontFamily(ConvertRecipeToPDF.FontFamilyBody);
    }

    private static bool AddEmphasis(TextDescriptor td, EmphasisInline emph)
    {
        if ((emph.DelimiterChar == '*' || emph.DelimiterChar == '_') && emph.DelimiterCount == 2) return AddExtendedEmphasis(td, emph, ElementType.Bold);
        if ((emph.DelimiterChar == '*' || emph.DelimiterChar == '_') && emph.DelimiterCount == 1) return AddExtendedEmphasis(td, emph, ElementType.Italic);
        // if (emph.DelimiterChar == '^' && emph.DelimiterCount == 1) return AddExtendedEmphasis(ElementType.Superscript);
        // if (emph.DelimiterChar == '~' && emph.DelimiterCount == 1) return AddExtendedEmphasis(ElementType.Subscript);

        // if (emph.DelimiterChar == '"' && emph.DelimiterCount == 2) return AddExtendedEmphasis(ElementType.Cite);
        // if (emph.DelimiterChar == '~' && emph.DelimiterCount == 2) return AddExtendedEmphasis(ElementType.Strike);
        // if (emph.DelimiterChar == '+' && emph.DelimiterCount == 2) return AddExtendedEmphasis(ElementType.Inserted);
        // if (emph.DelimiterChar == '=' && emph.DelimiterCount == 2) return AddExtendedEmphasis(ElementType.Marked);
        return false;
    }

    private static bool AddExtendedEmphasis(TextDescriptor td, EmphasisInline emph, ElementType elementType)
    {
        var literal = emph as ContainerInline;
        foreach (var item in emph)
        {
            if (item is LiteralInline literalInline)
            {
                var text = literalInline.Content.ToString();
                switch (elementType)
                {
                    case ElementType.Bold: td.Span(text).Bold(); break;
                    case ElementType.Italic: td.Span(text).Italic(); break;
                }

            }
        }
        return true;
    }

    private static string GetInlinePlainText(Inline i)
    {
        var res = new StringBuilder();
        PopulateInlinePlainText(i, res);
        return res.ToString();
    }

    private static void PopulateInlinePlainText(Inline i, StringBuilder res)
    {
        switch (i)
        {
            case LiteralInline literal:
                res.Append(literal.Content);
                break;

            case CodeInline code:
                res.Append(code.Content);
                break;

            case ContainerInline container:
                foreach (var ii in container)
                {
                    PopulateInlinePlainText(ii, res);
                }
                break;
        }
    }
}