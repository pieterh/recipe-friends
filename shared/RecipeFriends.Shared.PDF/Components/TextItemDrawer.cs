using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace RecipeFriends.Shared.PDF.Components;

internal class TextItemDrawer : ItemDrawer
{
    public override void Compose(IContainer container)
    {
        if (Item == null) throw new InvalidOperationException("The property Item should not be null");
        container.Row(row =>
                    {
                        row.Spacing(5);
                        row.AutoItem().Text($"{Index + 1}.");
                        row.RelativeItem().Text(Item.ToString());
                    });
    }
}