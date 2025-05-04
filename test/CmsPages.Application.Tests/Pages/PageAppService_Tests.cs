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

    [Fact]
    public async Task Should_Get_Page_Menu_Items()
    {
        // Act
        var result = await _pageAppService.GetPageMenuItemsAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
        result.ShouldContain(item => !string.IsNullOrWhiteSpace(item.DisplayName));
    }

    [Fact]
    public async Task Should_Get_Page_By_Route_Name()
    {
        // Arrange
        var created = await _pageAppService.CreateAsync(new CreateUpdatePageDto
        {
            Title = "A Random Page",
            RouteName = "a-random-page",
            Content = null,
            IsHomePage = false
        });

        // Act
        var result = await _pageAppService.GetByRouteNameAsync("a-random-page");

        // Assert
        result.ShouldNotBeNull();
        result?.Id.ShouldBe(created.Id);
        result?.Title.ShouldBe("A Random Page");
        result?.Content.ShouldBe(null);
    }

    [Fact]
    public async Task Should_Return_Null_If_Route_Name_Not_Found()
    {
        // Act
        var result = await _pageAppService.GetByRouteNameAsync("non-existent-route");

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task Should_Get_Home_Page()
    {
        // Arrange
        var homePage = await _pageAppService.CreateAsync(new CreateUpdatePageDto
        {
            Title = "Home Page",
            RouteName = "home",
            Content = "<b>Home</b>",
            IsHomePage = true
        });

        // Act
        var result = await _pageAppService.GetHomePageAsync();

        // Assert
        result.ShouldNotBeNull();
        result?.IsHomePage.ShouldBeTrue();
        result?.Id.ShouldBe(homePage.Id);
        result?.Content.ShouldBe("<b>Home</b>");
    }

    [Fact]
    public void Should_Decode_Html_Content()
    {
        // Arrange
        var encoded = "This &amp; that &lt;div&gt;";

        // Act
        var result = _pageAppService.DecodeHtmlContent(encoded);

        // Assert
        result.ShouldBe("This & that <div>");
    }

    [Fact]
    public void Should_Decode_And_Sanitize_Html_Content()
    {
        // Arrange
        var encodedHtml = "&lt;script&gt;alert(&quot;xss&quot;)&lt;/script&gt;&lt;div&gt;Hello&lt;/div&gt;";

        // Act
        var result = _pageAppService.GetDecodedAndSanitizedPageContentAsync(encodedHtml);

        // Assert
        result.ShouldContain("Hello");
        result.ShouldNotContain("script");
        result.ShouldNotContain("alert");
    }

    [Fact]
    public async Task Should_Allow_Null_Content()
    {
        // Act
        var result = await _pageAppService.CreateAsync(new CreateUpdatePageDto
        {
            Title = "Valid Title",
            RouteName = "valid-route-name",
            Content = null,  // Content is optional
            IsHomePage = false
        });

        // Assert
        result.ShouldNotBeNull();
        result.Content.ShouldBeNull();
    }

    [Fact]
    public async Task Should_Throw_When_Title_Exceeds_Max_Length()
    {
        // Act and Assert
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _pageAppService.CreateAsync(new CreateUpdatePageDto
            {
                Title = new string('a', 129),  // Title exceeds 128 characters
                RouteName = "another-route-name",
                Content = "Test Content",
                IsHomePage = false
            });
        });

        exception.ValidationErrors
            .ShouldContain(err => err.MemberNames.Any(mem => mem == "Title"));
    }

    [Fact]
    public async Task Should_Throw_When_RouteName_Exceeds_Max_Length()
    {
        // Act and Assert
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _pageAppService.CreateAsync(new CreateUpdatePageDto
            {
                Title = "Another Title",
                RouteName = new string('a', 129),  // RouteName exceeds 128 characters
                Content = "Test Content",
                IsHomePage = false
            });
        });

        exception.ValidationErrors
            .ShouldContain(err => err.MemberNames.Any(mem => mem == "RouteName"));
    }


}