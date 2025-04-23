using Volo.Abp.Modularity;

namespace CmsPages;

public abstract class CmsPagesApplicationTestBase<TStartupModule> : CmsPagesTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
