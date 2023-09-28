
using System.Text;

using RecipeFriends.Services;

namespace RecipeFriends;

public class DocumentService : IDocumentService
{
    private IRecipeService _recipeService;
    public DocumentService(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    public async Task<string> RecipeToMarkdownAsync(int id, CancellationToken cancellationToken)
    {
        var recipe = await _recipeService.GetRecipeDetailsAsync(id, cancellationToken);
        var recipeMarkdown = new StringBuilder();
        recipeMarkdown.AppendLine("# " + recipe.Title);
        recipeMarkdown.AppendLine();
        recipeMarkdown.AppendLine(recipe.ShortDescription);
        recipeMarkdown.AppendLine("***");
        recipeMarkdown.AppendLine("## Description");
        recipeMarkdown.AppendLine(recipe.Description);
        recipeMarkdown.AppendLine("***");
        recipeMarkdown.AppendLine("## Directions");
        recipeMarkdown.AppendLine(recipe.Directions);
        recipeMarkdown.AppendLine("***");
        recipeMarkdown.AppendLine("## Personal notes");
        recipeMarkdown.AppendLine(recipe.Notes);        
        return recipeMarkdown.ToString();
    }
    public async Task<IEnumerable<byte[]>> RecipeToImageAsync(int id, CancellationToken cancellationToken)
    {
        var recipeDetails = await _recipeService.GetRecipeDetailsAsync(id, cancellationToken).ConfigureAwait(true);
        var c = new Shared.PDF.ConvertRecipeToPDF();
        return await Task.FromResult(c.ToImage(recipeDetails)).ConfigureAwait(true);
    }

    public async Task<byte[]> RecipeToPDFAsync(int id, CancellationToken cancellationToken)
    {
        var recipeDetails = await _recipeService.GetRecipeDetailsAsync(id, cancellationToken).ConfigureAwait(true);
        var c = new Shared.PDF.ConvertRecipeToPDF();
        return await Task.FromResult(c.DoTest(recipeDetails)).ConfigureAwait(true);
    }

    // public async Task<string> RecipeToHtmlAsync(int id, CancellationToken cancellationToken)
    // {
    //     //var recipeMarkdown = await RecipeToMarkdownAsync(id, cancellationToken);       
    //     var recipe = await _recipeService.GetRecipeDetailsAsync(id, cancellationToken);
    //     var p = new MarkdownProcessor();
    //     // p.ToHtml(recipeMarkdown, out var recipeHtml);
    //     var recipeHtml = new StringBuilder();

    //     recipeHtml.AppendLine($"""
    //     <div class="d-sm-flex">        
    //         <div>          
    //             <h5 class="py-3 mb-0 h2">{recipe.Title}</h5>
    //         </div>
    //     </div>
    //     """);

    //     recipeHtml.AppendLine($"<div class=\"blog-detail\">");
    //     recipeHtml.AppendLine($"<hr/>");
    //     recipeHtml.AppendLine($"<p>{p.ToHtml(recipe.ShortDescription)}</p>");
    //     recipeHtml.AppendLine($"<br/>");
    //     recipeHtml.AppendLine($"<p>{p.ToHtml(recipe.Description)}</p>");
    //     recipeHtml.AppendLine($"<hr/>");
    //     recipeHtml.AppendLine($"<div class=\"row mt-0 mt-md-5\">");
    //     recipeHtml.AppendLine($"<div class=\"col-lg-8 col-md-7\">");
    //     recipeHtml.AppendLine($"""
    //         <div class="border-md-right pr-0 pr-lg-5"> 
    //             <ul class="list-unstyled component-list tstbite-svg">
    //                 <li>
    //                 <small>Prep Time</small>
    //                 <span>15 min</span>
    //                 </li>
    //                 <li>
    //                 <small>Cook Time</small>
    //                 <span>15 min</span>
    //                 </li>
    //                 <li>
    //                 <small>Servings</small>
    //                 <span>4 People</span>
    //                 </li>                
    //             </ul>            
    //     """); 
        
    //     recipeHtml.AppendLine($"<div class=\"mt-3 mt-md-5\"><h6>Directions</h6>{DirectionsHtml(recipe.Directions)}</div>");
    //     recipeHtml.AppendLine($"</div></div>");
    //     recipeHtml.AppendLine($"<div class=\"col-lg-4 col-md-5\">");
    //     recipeHtml.AppendLine($"<div class=\"rounded-12 bg-lightest-gray p-4\"><h6>Ingredients</h6><ul class=\"Nutrition-list list-unstyled\">{IngredientsHtml(recipe.Ingredients)}</ul></div>");
    //     recipeHtml.AppendLine($"<br/>");
    //     recipeHtml.AppendLine($"<div class=\"rounded-12 bg-lightest-gray p-4\"><h6>Equipment</h6><ul class=\"Nutrition-list list-unstyled\">{EquipmentHtml(new string[] {"skillet"})}</ul></div>");

        

    //     recipeHtml.AppendLine($"</div></div>");
    //     return recipeHtml.ToString();
    // }

    // private string DirectionsHtml(string directions){
    //     var b = new StringBuilder();
    //     b.AppendLine("<ul class=\"instruction-list list-unstyled\">");
    //     var lines = directions.Split('\n');
    //     int nr = 1;
    //     foreach(var line in lines){
    //         b.AppendLine($"<li><span>{nr}</span>{line}</span></li>");
    //         nr++;
    //     }
    //     b.AppendLine("</ul>");
    //     return b.ToString();
    // }

    // private string IngredientsHtml(IEnumerable<IngredientDetails> ingredients){
    //     var b = new StringBuilder();
    //     foreach(var i in ingredients){
    //         b.AppendLine($"<li><span>{i.Name}</span><span>{i.Amount}{i.Measurement.ToString()}</span></li>");
    //     }
    //     return b.ToString();
    // }

    // private string EquipmentHtml(IEnumerable<string> equipmentlist){
    //     var b = new StringBuilder();
    //     foreach(var e in equipmentlist){
    //         b.AppendLine($"<li><span>{e}</span></li>");
    //     }
    //     return b.ToString();
    // }


}
