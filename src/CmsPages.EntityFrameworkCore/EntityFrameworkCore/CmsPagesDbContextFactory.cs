using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CmsPages.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class CmsPagesDbContextFactory : IDesignTimeDbContextFactory<CmsPagesDbContext>
{
    public CmsPagesDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        CmsPagesEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<CmsPagesDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new CmsPagesDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../CmsPages.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
