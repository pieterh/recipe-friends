using System.IO.Compression;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using RecipeFriends.Data;
using RecipeFriends.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Recipe Friends",
        Version = "v1",
        Description = "Recipe Friends API.",
        TermsOfService = new Uri("https://example.com/terms"),
        License = new OpenApiLicense
        {
            Name = "BSD-3-Clause license",
            Url = new Uri("https://github.com/pieterh/recipe-friends/blob/86762db3c8d4f372fa80e4be0b5946d9cc731ef9/LICENSE.md"),
        }
    });

    // Enable annotations for Swagger
    options.EnableAnnotations();
    options.SupportNonNullableReferenceTypes();
    options.OperationFilter<SecurityRequirementsOperationFilter>();
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
builder.Services.AddControllers().AddJsonOptions(options =>
 options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.SmallestSize;
});

builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = new[] {
                        "application/font-woff2",
                        "application/javascript",
                        "application/json",
                        "application/octet-stream",
                        "application/wasm",
                        "application/xml",
                        "text/css",
                        "text/javascript",
                        "text/json",
                        "text/html",
                        "text/plain",
                };

    options.EnableForHttps = true;
});

builder.Services.AddDbContext<RecipeFriendsContext>(options => options.UseSqlite($"Data Source={RecipeFriendsContext.DbPath}"));
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
