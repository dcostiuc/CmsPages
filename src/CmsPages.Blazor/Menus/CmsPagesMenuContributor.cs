using System.Threading.Tasks;
using CmsPages.Localization;
using CmsPages.Permissions;
using CmsPages.MultiTenancy;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.UI.Navigation;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.TenantManagement.Blazor.Navigation;
using Volo.Abp.Identity.Blazor;

namespace CmsPages.Blazor.Menus;

public class CmsPagesMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<CmsPagesResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                CmsPagesMenus.Home,
                l["Menu:Home"],
                "/",
                icon: "fas fa-home",
                order: 1)
        );

        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 6;

        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
        administration.SetSubItemOrder(SettingManagementMenus.GroupName, 3);

        context.Menu.AddItem(
         new ApplicationMenuItem(
             "Pages",
             l["Menu:Pages"],
             icon: "fas fa-file-alt",
             url: "/pages"
         ).RequirePermissions(CmsPagesPermissions.Pages.Default)
        );

        return Task.CompletedTask;
    }
}
