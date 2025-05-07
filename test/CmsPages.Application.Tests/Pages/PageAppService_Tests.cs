using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;
using Volo.Abp;

namespace CmsPages.Pages;

public abstract class PageAppService_Tests<TStartupModule> : CmsPagesApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    protected readonly IPageAppService _pageAppService;

    protected PageAppService_Tests()
    {
        _pageAppService = GetRequiredService<IPageAppService>();
    }

    [Fact]
    public async Task Should_Get_List_Of_Pages()
    {
        //Act
        var result = await _pageAppService.GetListAsync(
            new PageFilterDto()
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
        var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _pageAppService.GetAsync(nonExistentId);
        });

        Assert.Equal(CmsPagesDomainErrorCodes.PageNotFound, exception.Code);
    }

    [Fact]
    public async Task Should_Update_Existing_Page()
    {
        // Arrange
        var pages = await _pageAppService.GetListAsync(new PageFilterDto());
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

        var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _pageAppService.UpdateAsync(nonExistentId, updatedDto);
        });

        Assert.Equal(CmsPagesDomainErrorCodes.PageUpdateFailed, exception.Code);
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
        var list = await _pageAppService.GetListAsync(new PageFilterDto());
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
        var allPages = await _pageAppService.GetListAsync(new PageFilterDto());
        foreach (var page in allPages.Items)
        {
            await _pageAppService.DeleteAsync(page.Id);
        }

        // Act
        var result = await _pageAppService.GetListAsync(new PageFilterDto());

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

    [Fact]
    public async Task Should_Throw_When_Creating_Page_With_Existing_RouteName()
    {
        // Arrange
        await _pageAppService.CreateAsync(new CreateUpdatePageDto
        {
            Title = "First Page",
            RouteName = "duplicate-route",
            Content = "<p>Page 1</p>"
        });

        // Act and Assert
        var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _pageAppService.CreateAsync(new CreateUpdatePageDto
            {
                Title = "Second Page",
                RouteName = "duplicate-route",
                Content = "<p>Page 2</p>"
            });
        });
        exception.Message.ShouldContain("A page with the route name");
        Assert.Equal(CmsPagesDomainErrorCodes.PageCreationFailedExistingRoute, exception.Code);
    }

    [Fact]
    public async Task Should_Throw_When_Updating_Page_To_Existing_RouteName()
    {
        // Arrange
        var page1 = await _pageAppService.CreateAsync(new CreateUpdatePageDto
        {
            Title = "Page One",
            RouteName = "first-route",
            Content = "Content 1"
        });

        var page2 = await _pageAppService.CreateAsync(new CreateUpdatePageDto
        {
            Title = "Page Two",
            RouteName = "another-route",
            Content = "Content 2"
        });

        // Act and Assert
        var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _pageAppService.UpdateAsync(page2.Id, new CreateUpdatePageDto
            {
                Title = "Page Two Edited",
                RouteName = "first-route", // Same as page1
                Content = "Updated Content"
            });
        });
        exception.Message.ShouldContain("already uses the route name");
        Assert.Equal(CmsPagesDomainErrorCodes.PageUpdateFailedExistingRoute, exception.Code);
    }

    [Fact]
    public async Task Should_Filter_By_Title()
    {
        // Arrange
        var titleToSearch = "My"; // we expect to get only 1 page - the one with title "My Page"

        // Act
        var result = await _pageAppService.GetListAsync(new PageFilterDto
        {
            Title = titleToSearch,
            MaxResultCount = 10
        });

        // Assert
        result.Items.ShouldNotBeEmpty();
        result.Items.All(p => p.Title.Contains(titleToSearch, StringComparison.InvariantCultureIgnoreCase)).ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Filter_By_RouteName()
    {
        // Arrange
        var routeToSearch = "another"; // we expect to get only 1 page - the one with route name "another-page"

        // Act
        var result = await _pageAppService.GetListAsync(new PageFilterDto
        {
            RouteName = routeToSearch,
            MaxResultCount = 10
        });

        // Assert
        result.Items.ShouldNotBeEmpty();
        result.Items.All(p => p.RouteName.Contains(routeToSearch, StringComparison.InvariantCultureIgnoreCase)).ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Return_Empty_If_No_Match()
    {
        var result = await _pageAppService.GetListAsync(new PageFilterDto
        {
            Title = "NonExistentTitleXYZ"
        });

        result.TotalCount.ShouldBe(0);
        result.Items.ShouldBeEmpty();
    }

    [Fact]
    public async Task Should_Support_Pagination()
    {
        // Act
        var result = await _pageAppService.GetListAsync(new PageFilterDto
        {
            MaxResultCount = 1,
            SkipCount = 0
        });

        // Assert
        result.Items.Count.ShouldBe(1);
        result.TotalCount.ShouldBeGreaterThan(1); // we seed 2 entries
    }

    [Fact]
    public async Task Should_Sort_By_Title_Ascending()
    {
        // Arrange
        var pages = new[]
        {
            new CreateUpdatePageDto { Title = "Zebra", RouteName = "zebra" },
            new CreateUpdatePageDto { Title = "Apple", RouteName = "apple" },
            new CreateUpdatePageDto { Title = "Monkey", RouteName = "monkey" },
        };

        foreach (var page in pages)
        {
            await _pageAppService.CreateAsync(page);
        }

        // Act
        var result = await _pageAppService.GetListAsync(new PageFilterDto
        {
            Sorting = "Title",
            MaxResultCount = 10
        });

        // Assert
        var sorted = result.Items.Select(p => p.Title).ToList();
        sorted.ShouldBe(new[] { "Another Page", "Apple", "Monkey", "My Page", "Zebra" }); // in this case it also includes the 2 pre-seeded pages
    }

    [Fact]
    public async Task Should_Sort_By_Title_Descending()
    {
        // Arrange
        var pages = new[]
        {
            new CreateUpdatePageDto { Title = "Zebra", RouteName = "zebra" },
            new CreateUpdatePageDto { Title = "Apple", RouteName = "apple" },
            new CreateUpdatePageDto { Title = "Monkey", RouteName = "monkey" },
        };

        foreach (var page in pages)
        {
            await _pageAppService.CreateAsync(page);
        }

        // Act
        var result = await _pageAppService.GetListAsync(new PageFilterDto
        {
            Sorting = "Title DESC",
            MaxResultCount = 10
        });

        // Assert
        var sorted = result.Items.Select(p => p.Title).ToList();
        var manuallySorted = sorted.OrderByDescending(t => t).ToList();

        sorted.ShouldBe(manuallySorted);
    }

    [Fact]
    public void Should_Convert_Markdown_To_Html()
    {
        // Arrange
        var markdown = "# Hello World\nThis is a *test*.";

        // Act
        var result = _pageAppService.ConvertMarkdownToHtml(markdown);

        // Assert
        result.ShouldContain("<h1", Case.Insensitive); // check there's an h1
        result.ShouldContain("Hello World", Case.Insensitive);
        result.ShouldContain("<p>This is a <em>test</em>.</p>", Case.Insensitive);
    }


    [Fact]
    public void Should_Sanitize_Html()
    {
        // Arrange
        var html = "<script>alert('xss')</script><div>Safe content</div>";

        // Act
        var result = _pageAppService.SanitizeHtml(html);

        // Assert
        result.ShouldContain("Safe content");
        result.ShouldNotContain("<script>");
        result.ShouldNotContain("alert");
    }

}