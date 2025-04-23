using Volo.Abp.Modularity;

namespace CmsPages;

[DependsOn(
    typeof(CmsPagesApplicationModule),
    typeof(CmsPagesDomainTestModule)
)]
public class CmsPagesApplicationTestModule : AbpModule
{

}
