using CmsPages.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace CmsPages.Permissions;

public class CmsPagesPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(CmsPagesPermissions.GroupName);

        var pagesPermission = myGroup.AddPermission(CmsPagesPermissions.Pages.Default, L("Permission:Pages"));
        pagesPermission.AddChild(CmsPagesPermissions.Pages.Create, L("Permission:Pages.Create"));
        pagesPermission.AddChild(CmsPagesPermissions.Pages.Edit, L("Permission:Pages.Edit"));
        pagesPermission.AddChild(CmsPagesPermissions.Pages.Delete, L("Permission:Pages.Delete"));

        //Define your own permissions here. Example:
        //myGroup.AddPermission(CmsPagesPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CmsPagesResource>(name);
    }
}
