using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using RecipeFriends.Data.Models;

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
            //DbPath = System.IO.Path.Join(folder, "recipefriends.db");
            DbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "recipefriends.db");
            Console.WriteLine(DbPath);
            Logger.Warn("No name for database file given. Defaults to database file {DbPath}", DbPath);
        }
        else
            Logger.Debug("Using database file {DbPath}", DbPath);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ArgumentNullException.ThrowIfNull(modelBuilder);

        // create instances of the converters beeing used
        var dateTimeConverter = new DateTimeToUtcConverter();
        var nullableDateTimeConverter = new NullableDateTimeToUtcConverter();
        var timeOnlyConverter = new TimeOnlyConverter();

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
                else if (property.ClrType == typeof(TimeOnly))
                {
                    property.SetValueConverter(timeOnlyConverter);
                }
            }
        }
    }
}

public class DateTimeToUtcConverter : ValueConverter<DateTime, DateTime>
{
    public DateTimeToUtcConverter()
        : base(
            v => Cvt(v),
            v => Cvt(v))
    { }

    private static DateTime Cvt(DateTime dt)
    {
        if (dt.Kind == DateTimeKind.Unspecified)
            dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
        else
            dt = dt.ToUniversalTime();
        return dt;
    }
}

public class NullableDateTimeToUtcConverter : ValueConverter<DateTime?, DateTime?>
{
    public NullableDateTimeToUtcConverter()
        : base(
            v => v.HasValue ? v.Value.ToUniversalTime() : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v)
    { }
}

public class TimeOnlyConverter : ValueConverter<TimeOnly, TimeSpan>
{
    public TimeOnlyConverter() : base(
            timeOnly => timeOnly.ToTimeSpan(),
            timeSpan => TimeOnly.FromTimeSpan(timeSpan))
    {
    }
}

public class TimeOnlyComparer : ValueComparer<TimeOnly>
{
    public TimeOnlyComparer() : base(
        (t1, t2) => t1.Ticks == t2.Ticks,
        t => t.GetHashCode())
    {
    }
}