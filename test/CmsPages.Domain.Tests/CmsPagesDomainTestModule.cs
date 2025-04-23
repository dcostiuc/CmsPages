using Volo.Abp.Modularity;

namespace CmsPages;

[DependsOn(
    typeof(CmsPagesDomainModule),
    typeof(CmsPagesTestBaseModule)
)]
public class CmsPagesDomainTestModule : AbpModule
{

}
