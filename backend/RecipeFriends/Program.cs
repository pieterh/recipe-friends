using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RecipeFriends.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Recipe Friends",
        Version = "v1",
        Description = "Description for your API.",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Url = new Uri("https://twitter.com/johndoe"),
        },
        License = new OpenApiLicense
        {
            Name = "Use under XYZ",
            Url = new Uri("https://example.com/license"),
        }
    });
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
