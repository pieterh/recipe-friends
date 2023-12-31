﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using RecipeFriends.Shared.Data.Models;

namespace RecipeFriends.Shared.Data;

[SuppressMessage("SonarLint", "S101", Justification = "Ignored intentionally")]
public class RecipeFriendsDbContext : DbContext
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    public static string DbPath { get; set; } = default!;
    public DbSet<Recipe> Recipes { get; set; } = default!;
    public DbSet<Image> Images { get; set; } = default!;

    public DbSet<Tag> Tags { get; set; } = default!;

    public DbSet<Equipment> Equipment { get; set; } = default!;

    public DbSet<Language> Languages { get; set; } = default!;

    public DbSet<Category> Catagories { get; set; } = default!;
    public DbSet<Measurement> Measurements { get; set; } = default!;

    public DbSet<MeasurementTranslation> MeasurementTranslations { get; set; } = default!;

    public DbSet<CategoryTranslation> CategoryTranslations { get; set; } = default!;

    public RecipeFriendsDbContext()
    {
        

        if (string.IsNullOrWhiteSpace(DbPath))
        {
            var folder = AppDomain.CurrentDomain.BaseDirectory;
            DbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Recipe Friends", "recipefriends.db");
            Logger.Warn("No name for database file given. Defaults to database file {DbPath}", DbPath);
        }
        else
            Logger.Debug("Using database file {DbPath}", DbPath);
    }

    public static readonly LoggerFactory MyLoggerFactory
        = new LoggerFactory(new[] {new NLogLoggerProvider ()});

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
#if DEBUG        
        optionsBuilder.EnableSensitiveDataLogging() ;
#endif        

        optionsBuilder.UseLoggerFactory(MyLoggerFactory);  //tie-up DbContext with LoggerFactory object
      
    }

    public void PerformInitialization()
    {
        try
        {
            Logger.Info("=> PerformInitialization");

            Logger.Info("Update database settings");
            this.Database.ExecuteSqlRaw("PRAGMA journal_mode = WAL;");
            this.Database.ExecuteSqlRaw("PRAGMA journal_size_limit = 50;");

            Logger.Info("Perform database migrations");
            this.Database.Migrate(); // Apply migrations
            
            Logger.Info("<= PerformInitialization");
        }
        catch (Exception ex)
        {
            // Log the exception or terminate the application based on your needs
            Logger.Error(ex, "Problem migrating database");
        }
    }

    public void PerformShutdown()
    {
        Logger.Info("=> PerformShutdown");
        Logger.Info("== SQLite checkpoint");
        var i1 = this.Database.ExecuteSqlRaw("pragma wal_checkpoint;");
        Logger.Info("== SQLite checkpoint {i}", i1);

        Logger.Info("== SQLite Optimize");
        var i2 = this.Database.ExecuteSqlRaw("pragma optimize;");
        Logger.Info("== SQLite Optimize {i}", i2);

        Logger.Info("<= PerformShutdown");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ArgumentNullException.ThrowIfNull(modelBuilder);

        // create instances of the converters beeing used
        var dateTimeConverter = new DateTimeToUtcConverter();
        var nullableDateTimeConverter = new NullableDateTimeToUtcConverter();
        var timeOnlyConverter = new TimeOnlyConverter();

        modelBuilder.Entity<Language>()
            .Property(e => e.Status)
            .HasConversion<int>();
        modelBuilder.Entity<Category>()
            .Property(e => e.Status)
            .HasConversion<int>();
        modelBuilder.Entity<Ingredient>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();

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