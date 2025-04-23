using CmsPages.Localization;
using Volo.Abp.AspNetCore.Components;

namespace CmsPages.Blazor;

public abstract class CmsPagesComponentBase : AbpComponentBase
{
    protected CmsPagesComponentBase()
    {
        LocalizationResource = typeof(CmsPagesResource);
    }
}
