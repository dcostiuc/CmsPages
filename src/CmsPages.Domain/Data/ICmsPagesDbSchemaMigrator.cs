using System.Threading.Tasks;

namespace CmsPages.Data;

public interface ICmsPagesDbSchemaMigrator
{
    Task MigrateAsync();
}
