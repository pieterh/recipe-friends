﻿using System.Xml;
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
using System.Reflection;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace RecipeFriends;

public static class MauiProgram
{
	private static IServiceProvider services;

	public static MauiApp CreateMauiApp()
	{
		SetupNLog();
		var logger = NLog.LogManager.GetCurrentClassLogger();
		DotNetInfo.Info(logger);

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureLifecycleEvents(AppLifecycle =>
			{
				logger.Info("ConfigureLifecycleEvents");
				
#if MACCATALYST || IOS
				AppLifecycle.AddiOS(ios => ios.DidEnterBackground((a) => {
					var logger = NLog.LogManager.GetCurrentClassLogger();
					logger.Error("DidEnterBackground");
					Console.WriteLine("DidEnterBackground");
				}));
				AppLifecycle.AddiOS(ios => ios.WillTerminate((a) => {
					var logger = NLog.LogManager.GetCurrentClassLogger();
					logger.Error("WillTerminate");
					Console.WriteLine("WillTerminate");
					var ctx = new RecipeFriendsDbContext();
					ctx.Checkpoint();
				}));				
#elif WINDOWS
				AppLifecycle.AddWindows(windows => windows.OnClosed((window, args) => {
					logger.Error("OnClosed");
					Console.WriteLine("OnClosed");
					var ctx = new RecipeFriendsDbContext();
					ctx.Checkpoint();
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

		AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
		{
			var logger = NLog.LogManager.GetCurrentClassLogger();
			logger.Error($"In UnhandledException {Environment.NewLine} {args.ExceptionObject}");
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

	private static void SetupNLog()
	{
		LogManager.Setup().RegisterMauiLog();
		// set first a failsafe configuration from program logic

		var cfg1 = new LoggingConfiguration();
		cfg1.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, new NLog.Targets.ConsoleTarget("Console"));
		// apply configuration
		LogManager.Configuration = cfg1;

		LogManager.Setup().RegisterMauiLog().LoadConfiguration(c => c.ForLogger(NLog.LogLevel.Debug).WriteToMauiLog());
		var l = LogManager.GetCurrentClassLogger();

		l.Info("tst");
		try
		{
			// try to load from external configuration file
			//var assembly = typeof(MauiProgram).Assembly;
			//using var stream = FileSystem.OpenAppPackageFileAsync("NLog.txt").GetAwaiter().GetResult();
			using var stream = GetEmbeddedResourceStream(typeof(MauiProgram).Assembly, "NLog.config");
			var cfg = new XmlLoggingConfiguration(XmlReader.Create(stream), null);
			if (cfg.InitializeSucceeded ?? false)
			{
				LogManager.Configuration = cfg;
			}
			else
			{
				var logger = NLog.LogManager.GetCurrentClassLogger();
				logger.Error("There was a problem initializing the nlog configuration file.");
			}
		}
		catch (Exception e)
		{
			var logger = NLog.LogManager.GetCurrentClassLogger();
			logger.Error(e, "There was a problem initializing the nlog configuration file.");
		}
	}

	public static Stream GetEmbeddedResourceStream(Assembly assembly, string resourceFileName)
	{
		var resourcePaths = assembly.GetManifestResourceNames()
		  .Where(x => x.EndsWith(resourceFileName, StringComparison.OrdinalIgnoreCase))
		  .ToList();
		if (resourcePaths.Count == 1)
		{
			return assembly.GetManifestResourceStream(resourcePaths.Single());
		}
		return null;
	}
}

