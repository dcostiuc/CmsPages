using Volo.Abp.Settings;

namespace CmsPages.Settings;

public class CmsPagesSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(CmsPagesSettings.MySetting1));
    }
}
