using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Localization;
using MudBlazor;

internal class MudLocalizerImplementation : MudLocalizer
{
    private Dictionary<string, string> _localization;

    public MudLocalizerImplementation()
    {
        _localization = new()
        {
            { "MudDataGrid.is empty", "ist leer" },
            { "MudDataGrid.is not empty", "ist nicht leer" },
            { "MudDataGrid.contains", "enthält" },
            { "MudDataGrid.not contains", "enthält nicht" },
            { "MudDataGrid.FilterValue", "Filterwert"}
        };
    }
    
    public override LocalizedString this[string key]
    {
        get
        {
            var currentCulture = Thread.CurrentThread.CurrentUICulture.Parent.TwoLetterISOLanguageName;
            if (currentCulture.Equals("de", StringComparison.InvariantCultureIgnoreCase)
                && _localization.TryGetValue(key, out var res))
            {
                return new(key, res);
            }
            else
            {
                return new(key, key, true);
            }
        }
    }
}