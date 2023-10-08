
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace RecipeFriends.Shared.PDF.Components;

internal class InfoBoxList<T>(string title, IEnumerable<object>  items)  : IComponent where T : ItemDrawer, new()
{
    public string Title { get; } = title;
    public IEnumerable<object>  Items { get; } = items;

    private const string BackgroundColor  = "#F9F9F9";//F9F9F9

    public void Compose(IContainer container)
    {
        bool debugOn = false;
        container.Decoration(decoration => { 
            decoration.Before(before => {
                before             
                    .ShowOnce()
                    .ExtendHorizontal()
                    .Background(BackgroundColor)
                    .Column(col => {                                                        
                        col.Item().Element(x => debugOn ? x.DebugArea() : x)
                            .PaddingTop(10, Unit.Point)
                            .PaddingLeft(10, Unit.Point)
                            .PaddingRight(10, Unit.Point)
                            .PaddingBottom(-10, Unit.Point) // not clear why I need to fix it like this
                            .Text(txt =>{
                                txt.DefaultTextStyle(x => x.FontSize(ConvertRecipeToPDF.FontSizeH2).FontFamily(ConvertRecipeToPDF.FontFamilyH2));
                                txt.Line(Title).Bold();
                        });                                                        
                    });
            });
            decoration.Content(content => {                                                
                content
                    .ShowOnce()
                    .ExtendHorizontal()
                    .Background(BackgroundColor)  
                    .PaddingBottom(10, Unit.Point)
                    .Element(x => debugOn ? x.DebugArea() : x)                                   
                    .Column(col => {                                              
                        foreach (var (item, index) in Items.Select((value, idx) => (value, idx)))
                        {
                            var t = new T
                            {
                                Item = item,
                                Index = index
                            };
                            col.Item()                            
                                .PaddingLeft(10, Unit.Point)
                                .PaddingRight(10, Unit.Point)
                                .DefaultTextStyle(x => x.FontSize(ConvertRecipeToPDF.FontSizeBody).FontFamily(ConvertRecipeToPDF.FontFamilyBody))
                                .Component(t);
                        }
                    });
            });
        });
    }
}
