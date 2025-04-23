using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace CmsPages.Pages;

public abstract class PageAppService_Tests<TStartupModule> : CmsPagesApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IPageAppService _PageAppService;

    protected PageAppService_Tests()
    {
        _PageAppService = GetRequiredService<IPageAppService>();
    }

    [Fact]
    public async Task Should_Get_List_Of_Pages()
    {
        //Act
        var result = await _PageAppService.GetListAsync(
            new PagedAndSortedResultRequestDto()
        );

        //Assert
        result.TotalCount.ShouldBeGreaterThan(0);
        result.Items.ShouldContain(b => b.Title == "My Page");
    }

    [Fact]
    public async Task Should_Create_A_Valid_Page()
    {
        //Act
        var result = await _PageAppService.CreateAsync(
            new CreateUpdatePageDto
            {
                Title = "New test Page 42",
                RouteName = "new-test-page-42",
                Content = "This is a test page's content.",
                IsHomePage = false
            }
        );

        //Assert
        result.Id.ShouldNotBe(Guid.Empty);
        result.Title.ShouldBe("New test Page 42");
    }
    
    [Fact]
    public async Task Should_Not_Create_A_Page_Without_Title()
    {
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _PageAppService.CreateAsync(
                new CreateUpdatePageDto
                {
                    Title = "",
                    RouteName = "new-test-page-42",
                    Content = "This is a test page's content.",
                    IsHomePage = false
                }
            );
        });

        exception.ValidationErrors
            .ShouldContain(err => err.MemberNames.Any(mem => mem == "Title"));
    }
}