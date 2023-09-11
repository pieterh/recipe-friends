using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SQLite;

using RecipeFriends.Data;
using RecipeFriends.Services;



namespace RecipeFriends;

public static class MauiProgram
{
	private static SQLiteAsyncConnection _dbConnection;

	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif
		//builder.Services.AddDbContext<RecipeFriendsContext>(options => options.UseSqlite($"Data Source={RecipeFriendsContext.DbPath}"));

        AppDomain.CurrentDomain.UnhandledException += (sender, args) => 
        {
			Console.WriteLine("sadfas");
        };
        AppDomain.CurrentDomain.FirstChanceException += (sender, args) =>
        {
            Console.WriteLine("In FirstChanceException Handler");
            Console.WriteLine($"{args.Exception.Message}");
            Console.WriteLine($"==");
           // Console.WriteLine($"{args.Exception.ToString()}");

        };

		builder.Services.AddSingleton<RecipeFriendsContext>();
		builder.Services.AddSingleton<IRecipeService, RecipeService > ();
		builder.Services.AddSingleton<IDocumentService, DocumentService > ();
		builder.Services.AddSingleton<WeatherForecastService>();

		bool isOSX = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX);

		Console.WriteLine(System.Runtime.InteropServices.RuntimeInformation.OSDescription);
		Console.WriteLine(System.Runtime.InteropServices.RuntimeInformation.OSArchitecture);


		var app = builder.Build();

		using (var scope = app.Services.CreateScope())
		{
			var services = scope.ServiceProvider;
			try
			{
				var context = services.GetRequiredService<RecipeFriendsContext>();
				context.Database.Migrate(); // Apply migrations
			}
			catch (Exception ex)
			{
				// Log the exception or terminate the application based on your needs
			}
		}
return app;
	}
}

