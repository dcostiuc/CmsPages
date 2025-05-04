using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace CmsPages.Pages;

public abstract class PageAppService_Tests<TStartupModule> : CmsPagesApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IPageAppService _pageAppService;

    protected PageAppService_Tests()
    {
        _pageAppService = GetRequiredService<IPageAppService>();
    }

    [Fact]
    public async Task Should_Get_List_Of_Pages()
    {
        //Act
        var result = await _pageAppService.GetListAsync(
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
        var result = await _pageAppService.CreateAsync(
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
            await _pageAppService.CreateAsync(
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

    [Fact]
    public async Task Should_Not_Create_A_Page_Without_RouteName()
    {
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _pageAppService.CreateAsync(
                new CreateUpdatePageDto
                {
                    Title = "Valid Title",
                    RouteName = "",
                    Content = "This is a test page's content.",
                    IsHomePage = false
                }
            );
        });

        exception.ValidationErrors
            .ShouldContain(err => err.MemberNames.Any(mem => mem == "RouteName"));
    }

    [Fact]
    public async Task Should_Get_A_Page_By_Id()
    {
        // Arrange
        var createdPage = await _pageAppService.CreateAsync(
            new CreateUpdatePageDto
            {
                Title = "Page to Get",
                RouteName = "page-to-get",
                Content = "Some content",
                IsHomePage = false
            });

        // Act
        var result = await _pageAppService.GetAsync(createdPage.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdPage.Id);
        result.Title.ShouldBe("Page to Get");
    }

    [Fact]
    public async Task Should_Throw_When_Page_Not_Found()
    {
        // Arrange: use a new GUID that doesn't exist in the database
        var nonExistentId = Guid.NewGuid();

        // Act and Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
        {
            await _pageAppService.GetAsync(nonExistentId);
        });
    }

    [Fact]
    public async Task Should_Update_Existing_Page()
    {
        // Arrange
        var pages = await _pageAppService.GetListAsync(new PagedAndSortedResultRequestDto());
        var pageToUpdate = pages.Items.First();

        var updatedDto = new CreateUpdatePageDto
        {
            Title = "Updated Title",
            RouteName = "updated-route-name",
            Content = "Updated content.",
            IsHomePage = false
        };

        // Act
        var result = await _pageAppService.UpdateAsync(pageToUpdate.Id, updatedDto);

        // Assert
        result.Id.ShouldBe(pageToUpdate.Id);
        result.Title.ShouldBe("Updated Title");
        result.Content.ShouldBe("Updated content.");
    }

    [Fact]
    public async Task Should_Throw_When_Updating_Nonexistent_Page()
    {
        var nonExistentId = Guid.NewGuid();

        var updatedDto = new CreateUpdatePageDto
        {
            Title = "Updated Title",
            RouteName = "updated-route-name",
            Content = "Updated content.",
            IsHomePage = false
        };

        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
        {
            await _pageAppService.UpdateAsync(nonExistentId, updatedDto);
        });
    }

    [Fact]
    public async Task Should_Delete_Existing_Page()
    {
        // Arrange
        var newPageToBeDeleted = await _pageAppService.CreateAsync(new CreateUpdatePageDto
        {
            Title = "Delete Me",
            RouteName = "delete-me",
            Content = "Temp content",
            IsHomePage = false
        });

        // Act
        await _pageAppService.DeleteAsync(newPageToBeDeleted.Id);

        // Assert
        var list = await _pageAppService.GetListAsync(new PagedAndSortedResultRequestDto());
        list.Items.ShouldNotContain(p => p.Id == newPageToBeDeleted.Id);
    }

    [Fact]
    public async Task Should_Not_Throw_When_Deleting_Nonexistent_Page()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act and Assert
        var exception = await Record.ExceptionAsync(() => _pageAppService.DeleteAsync(nonExistentId));
        exception.ShouldBeNull();
    }


    [Fact]
    public async Task Should_Return_Empty_List_When_No_Pages_Exist()
    {
        // Arrange: make sure the db has no pages by deleting them all, at least for this test
        var allPages = await _pageAppService.GetListAsync(new PagedAndSortedResultRequestDto());
        foreach (var page in allPages.Items)
        {
            await _pageAppService.DeleteAsync(page.Id);
        }

        // Act
        var result = await _pageAppService.GetListAsync(new PagedAndSortedResultRequestDto());

        // Assert
        result.TotalCount.ShouldBe(0);
        result.Items.ShouldBeEmpty();
    }

}