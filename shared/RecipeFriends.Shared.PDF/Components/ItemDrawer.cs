using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace RecipeFriends.Shared.PDF.Components;

internal abstract class ItemDrawer : IComponent {
    
    internal ItemDrawer() { }

    public object? Item {get; set;}
    public int Index {get; set;}

    public abstract void Compose(IContainer container);
}
