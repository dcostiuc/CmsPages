using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace CmsPages.Data;

/* This is used if database provider does't define
 * ICmsPagesDbSchemaMigrator implementation.
 */
public class NullCmsPagesDbSchemaMigrator : ICmsPagesDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
