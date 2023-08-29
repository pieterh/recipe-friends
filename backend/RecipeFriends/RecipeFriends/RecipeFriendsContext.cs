using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RecipeFriends.Controllers;
using RecipeFriends.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;

namespace RecipeFriends.Data;

[SuppressMessage("SonarLint", "S101", Justification = "Ignored intentionally")]
public class RecipeFriendsContext : DbContext
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public static string DbPath { get; set; } = default!;
    public DbSet<Recipe> Recipes { get; set; } = default!;
    public DbSet<Tag> Tags { get; set; } = default!;

    public RecipeFriendsContext()
    {
        if (string.IsNullOrWhiteSpace(DbPath))
        {
            var folder = AppDomain.CurrentDomain.BaseDirectory;
            DbPath = System.IO.Path.Join(folder, "recipefriends.db");
            Logger.Warn("No name for database file given. Defaults to database file {DbPath}", DbPath);
        }
        else
            Logger.Debug("Using database file {DbPath}", DbPath);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={DbPath}");

    internal static DateTime cvt(DateTime dt)
    {
        if (dt.Kind == DateTimeKind.Unspecified)
            dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
        else
            dt = dt.ToUniversalTime();
        return dt;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ArgumentNullException.ThrowIfNull(modelBuilder);

        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => cvt(v),
                v => cvt(v)
            );

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue ? v.Value.ToUniversalTime() : v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v
            );

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.IsKeyless)
            {
                continue;
            }

            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullableDateTimeConverter);
                }
            }
        }

        //modelBuilder.Entity<RecipeTag>()
        //    .HasKey(rt => new { rt.RecipeId, rt.TagId }); // Composite key

        //modelBuilder.Entity<RecipeTag>()
        //    .HasOne(rt => rt.Recipe)
        //    .WithMany(r => r.RecipeTags)
        //    .HasForeignKey(rt => rt.RecipeId);

        //modelBuilder.Entity<RecipeTag>()
        //    .HasOne(rt => rt.Tag)
        //    .WithMany(t => t.RecipeTags)
        //    .HasForeignKey(rt => rt.TagId);
    }
}

