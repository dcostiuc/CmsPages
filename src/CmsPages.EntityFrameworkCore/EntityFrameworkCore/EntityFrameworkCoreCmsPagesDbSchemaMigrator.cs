using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CmsPages.Data;
using Volo.Abp.DependencyInjection;

namespace CmsPages.EntityFrameworkCore;

public class EntityFrameworkCoreCmsPagesDbSchemaMigrator
    : ICmsPagesDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreCmsPagesDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the CmsPagesDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<CmsPagesDbContext>()
            .Database
            .MigrateAsync();
    }
}
