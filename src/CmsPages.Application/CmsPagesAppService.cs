using CmsPages.Localization;
using Volo.Abp.Application.Services;

namespace CmsPages;

/* Inherit your application services from this class.
 */
public abstract class CmsPagesAppService : ApplicationService
{
    protected CmsPagesAppService()
    {
        LocalizationResource = typeof(CmsPagesResource);
    }
}
