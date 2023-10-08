using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using RecipeFriends.Shared.DTO;

namespace RecipeFriends.Shared.PDF.Components;

internal class IngredientItemDrawer : ItemDrawer
{
    private const string BorderColor = "#D3D3D3";//E8E8E8
    public override void Compose(IContainer container)
    {
        if (Item == null) throw new InvalidOperationException("The property Item should not be null");
        var ingredient = Item as IngredientDetails;
        if (ingredient == null) throw new InvalidOperationException("The property Item should be of type IngredientDetails");
        container.BorderColor(BorderColor).BorderBottom(0.5f, Unit.Point).Inlined(inlined => {    
                    inlined.Item().Row(row =>
                    {    
                        row.Spacing(5);
                        row.AutoItem().Text($"{Index + 1}.");
                        row.RelativeItem().AlignLeft().Text($"{ingredient.Name}" );
                        row.RelativeItem().AlignRight().Text($"{ingredient.Amount} {ingredient.Measurement.Name}" );
                    });
        });
    }
}