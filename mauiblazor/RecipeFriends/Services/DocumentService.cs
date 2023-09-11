
using MarkdownPdf;
using PdfSharpCore.Fonts;

using RecipeFriends.Services;

namespace RecipeFriends;

public class DocumentService : IDocumentService
{
    private IRecipeService _recipeService;
    public DocumentService(IRecipeService recipeService)
    {
        _recipeService = recipeService;

        		GlobalFontSettings.FontResolver = new MyFontResolver();
    }

    public async Task<byte[]> RecipeToPDFAsync(int id, CancellationToken cancellationToken)
    {
        var recipe = await _recipeService.GetRecipeDetailsAsync(id, cancellationToken);

        var markdown = recipe.Directions;

        MarkdownPdfGenerator generator = new();

        generator.LeftMargin = Unit.FromInch(0.5);
        generator.RightMargin = Unit.FromInch(0.5);
        generator.TopMargin = Unit.FromInch(0.5);
        generator.BottomMargin = Unit.FromInch(0.5);       

        generator.GeneratePdf(markdown);

        var b = Array.Empty<byte>();

        using(var stream = new MemoryStream()){
            generator.Save(stream, false);
            b = stream.ToArray();
        }

        return b;
    }
}
