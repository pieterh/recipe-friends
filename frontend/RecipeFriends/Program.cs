using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RecipeFriends;
using RecipeFriends.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

#if DEBUG
var baseAddres = new Uri("http://localhost:5201");
#else
var baseAddres =  new Uri(builder.HostEnvironment.BaseAddress);
#endif

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddHttpClient("ServerAPI", client => client.BaseAddress = baseAddres);

builder.Services.AddScoped<IRecipeService, RecipeService>((sp) =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("ServerAPI");
    var instance = ActivatorUtilities.CreateInstance<RecipeService>(sp, httpClient);
    return instance;
});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
