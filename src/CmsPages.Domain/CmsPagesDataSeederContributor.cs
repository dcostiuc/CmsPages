using System;
using System.Threading.Tasks;
using CmsPages.Pages;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace CmsPages;

public class CmsPagesDataSeederContributor
    : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Page, Guid> _pageRepository;

    public CmsPagesDataSeederContributor(IRepository<Page, Guid> pageRepository)
    {
        _pageRepository = pageRepository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _pageRepository.GetCountAsync() <= 0)
        {
            await _pageRepository.InsertAsync(
                new Page
                {
                    Title = "My Page",
                    RouteName = "my-page",
                    Content = "Here is some content.",
                    IsHomePage = true
                },
                autoSave: true
            );

            await _pageRepository.InsertAsync(
                new Page
                {
                    Title = "Another Page",
                    RouteName = "another-page",
                    Content = "Some more content",
                    IsHomePage = false
                },
                autoSave: true
            );
        }
    }
}