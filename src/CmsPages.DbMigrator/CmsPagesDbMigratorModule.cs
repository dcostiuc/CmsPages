using CmsPages.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace CmsPages.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(CmsPagesEntityFrameworkCoreModule),
    typeof(CmsPagesApplicationContractsModule)
)]
public class CmsPagesDbMigratorModule : AbpModule
{
}
