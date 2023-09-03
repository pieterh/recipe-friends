using System.IO.Compression;
using System.Reflection;
using System.Text.Json.Serialization;
using Asp.Versioning.Conventions;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RecipeFriends.Data;
using RecipeFriends.Swagger;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
builder.Services
    .AddApiVersioning(options =>
    {
        // indicating whether a default version is assumed when a client does
        // does not provide an API version.
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    })
    .AddApiExplorer(options =>
    {
        // Add the versioned API explorer, which also adds IApiVersionDescriptionProvider service
        // note: the specified format code will format the version as "'v'major[.minor][-status]"
        options.GroupNameFormat = "'v'VVV";

        // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
        // can also be used to control the format of the API version in route templates
        options.SubstituteApiVersionInUrl = true;
    });

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
    options.OperationFilter<SwaggerDefaultValues>();

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddApiVersioning(
                    options =>
                    {
                        // reporting api versions will return the headers
                        // "api-supported-versions" and "api-deprecated-versions"
                        options.ReportApiVersions = true;
                    })
                .AddMvc(
                    options =>
                    {
                        // automatically applies an api version based on the name of
                        // the defining controller's namespace
                        options.Conventions.Add(new VersionByNamespaceConvention());
                    });



builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

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
#if DEBUG
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "_myAllowSpecificOrigins",
          builder => {
              builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                ;
          });
});


#endif
        

var app = builder.Build();

// put this between UseRouting and map of controllers / swagger
// and CORS needs to be before response caching
app.UseCors("_myAllowSpecificOrigins");

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
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();

        // Build a swagger endpoint for each discovered API version
        foreach (var description in descriptions)
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
