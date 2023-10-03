using System.Xml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

using MudBlazor.Services;
using CommunityToolkit.Maui;

using NLog;
using NLog.Config;
using NLog.Extensions.Logging;

using RecipeFriends.Services;
using RecipeFriends.Shared.Data;

namespace RecipeFriends;

public static class MauiProgram
{
	private static IServiceProvider services;
	public static MauiApp CreateMauiApp()
	{
		LogManager.Configuration = new LoggingConfiguration();
		var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
		LogManager.Configuration.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, logconsole);

		// var t = new MainLayout();
		Console.WriteLine("OS Version: {0}", Environment.OSVersion.ToString());

		Console.WriteLine("Version: {0}", Environment.Version.ToString());

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureLifecycleEvents(AppLifecycle =>
			{
				var logger = NLog.LogManager.GetCurrentClassLogger();
				logger.Info("ConfigureLifecycleEvents");
#if MACCATALYST
				AppLifecycle.AddiOS(ios => ios.DidEnterBackground((a) => {
					var logger = NLog.LogManager.GetCurrentClassLogger();
					logger.Error("DidEnterBackground");
					Console.WriteLine("DidEnterBackground");
				}));
				AppLifecycle.AddiOS(ios => ios.WillTerminate((a) => {
					var logger = NLog.LogManager.GetCurrentClassLogger();
					logger.Error("WillTerminate");
					Console.WriteLine("WillTerminate");
				}));
#endif
			})
			.UseMauiCommunityToolkit(options =>
				{
					options.SetShouldSuppressExceptionsInConverters(false);
					options.SetShouldSuppressExceptionsInBehaviors(false);
					options.SetShouldSuppressExceptionsInAnimations(false);
				})
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
		builder.Logging.ClearProviders();
		builder.Logging.AddNLog();
#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug().AddFilter("Microsoft", Microsoft.Extensions.Logging.LogLevel.Warning);
#endif

		var assembly = typeof(MauiProgram).Assembly;
		using var stream = FileSystem.OpenAppPackageFileAsync("NLog.txt").GetAwaiter().GetResult();
		LogManager.Configuration = new XmlLoggingConfiguration(XmlReader.Create(stream), null);

		var logger = NLog.LogManager.GetCurrentClassLogger();

		AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
		{
			Console.WriteLine("sadfas");
			var logger = NLog.LogManager.GetCurrentClassLogger();
			logger.Error($"In FirstChanceException {Environment.NewLine} {args.ExceptionObject}");
			logger.Error(args.ExceptionObject as Exception, $"-");
		};
		AppDomain.CurrentDomain.FirstChanceException += (sender, args) =>
		{
			var logger = NLog.LogManager.GetCurrentClassLogger();
			logger.Error(args.Exception, $"In FirstChanceException {Environment.NewLine}");
		};

		builder.Services.AddMudServices();

		builder.Services.AddDbContext<RecipeFriendsDbContext>();
		builder.Services.AddSingleton<IRecipeService, RecipeService>();
		builder.Services.AddSingleton<IDocumentService, DocumentService>();
		builder.Services.AddSingleton<Data.WeatherForecastService>();

		bool isOSX = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX);

		Console.WriteLine(System.Runtime.InteropServices.RuntimeInformation.OSDescription);
		Console.WriteLine(System.Runtime.InteropServices.RuntimeInformation.OSArchitecture);

		FontSupport.Setup();

		var app = builder.Build();

		using (var scope = app.Services.CreateScope())
		{
			services = scope.ServiceProvider;
			try
			{
				var context = services.GetRequiredService<RecipeFriendsDbContext>();
				context.Database.Migrate(); // Apply migrations
			}
			catch (Exception ex)
			{
				// Log the exception or terminate the application based on your needs
				logger.Error(ex, "Problem migrating database");
			}
		}
		return app;
	}


}

