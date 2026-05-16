using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Sid.DbFieldManager.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class DbFieldManagerDbContextFactory : IDesignTimeDbContextFactory<DbFieldManagerDbContext>
{
    public DbFieldManagerDbContext CreateDbContext(string[] args)
    {
        DbFieldManagerEfCoreEntityExtensionMappings.Configure();

        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<DbFieldManagerDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));

        return new DbFieldManagerDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Sid.DbFieldManager.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
