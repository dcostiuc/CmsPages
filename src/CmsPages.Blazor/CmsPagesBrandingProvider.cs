using Microsoft.Extensions.Localization;
using CmsPages.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace CmsPages.Blazor;

[Dependency(ReplaceServices = true)]
public class CmsPagesBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<CmsPagesResource> _localizer;

    public CmsPagesBrandingProvider(IStringLocalizer<CmsPagesResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
