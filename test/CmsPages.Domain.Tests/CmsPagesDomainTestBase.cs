using Volo.Abp.Modularity;

namespace CmsPages;

/* Inherit from this class for your domain layer tests. */
public abstract class CmsPagesDomainTestBase<TStartupModule> : CmsPagesTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
