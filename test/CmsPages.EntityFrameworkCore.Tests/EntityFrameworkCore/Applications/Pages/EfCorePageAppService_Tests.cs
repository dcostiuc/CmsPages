using System.Threading.Tasks;
using CmsPages.Pages;
using Xunit;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using System;
using Volo.Abp.Users;

namespace CmsPages.EntityFrameworkCore.Applications.Pages;

[Collection(CmsPagesTestConsts.CollectionDefinitionName)]
public class EfCorePageAppService_Tests : PageAppService_Tests<CmsPagesEntityFrameworkCoreTestModule>
{
    private readonly IRepository<Page, Guid> _pageRepository;

    public EfCorePageAppService_Tests()
    {
        _pageRepository = GetRequiredService<IRepository<Page, Guid>>();
    }

    [Fact]
    public async Task Should_Have_Audit_Fields_On_Creation()
    {
        // Arrange
        var createPageDto = new CreateUpdatePageDto
        {
            Title = "Audit Test Page",
            RouteName = "audit-test-page",
            Content = "Some content",
            IsHomePage = false
        };

        // Act
        var createdPage = await _pageAppService.CreateAsync(createPageDto);

        // Assert
        createdPage.ShouldNotBeNull();
        createdPage.CreationTime.ShouldBeGreaterThan(DateTime.MinValue);
        createdPage.CreatorId.ShouldNotBeNull();
    }



}