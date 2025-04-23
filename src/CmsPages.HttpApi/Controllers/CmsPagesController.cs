using CmsPages.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace CmsPages.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class CmsPagesController : AbpControllerBase
{
    protected CmsPagesController()
    {
        LocalizationResource = typeof(CmsPagesResource);
    }
}
