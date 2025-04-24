

using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;
using CmsPages.Pages;
using Microsoft.Extensions.DependencyInjection;
public class PageMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name != StandardMenus.Main) return;

        var pageAppService = context.ServiceProvider.GetRequiredService<IPageAppService>();
        var pageMenuItems = await pageAppService.GetPageMenuItemsAsync();

        foreach (var item in pageMenuItems)
        {
            context.Menu.AddItem(
                new ApplicationMenuItem(item.Name, item.DisplayName, item.Url)
            );
        }
    }
}
