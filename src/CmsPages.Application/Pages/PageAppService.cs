using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using CmsPages.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using System.Linq.Dynamic.Core;
using System.Web;
using Ganss.Xss;
using Volo.Abp.Uow;
using CmsPages.Helpers;
using Volo.Abp;
using Microsoft.Extensions.Logging;
using Markdig;

namespace CmsPages.Pages;

[Authorize(CmsPagesPermissions.Pages.Default)]
public class PageAppService : ApplicationService, IPageAppService
{
    private readonly IRepository<Page, Guid> _pageRepository;
    private readonly ISlugHelper _slugHelper;

    public PageAppService(IRepository<Page, Guid> repository, ISlugHelper slugHelper)
    {
        _pageRepository = repository;
        _slugHelper = slugHelper;
    }

    [AllowAnonymous]
    public async Task<PageDto> GetAsync(Guid id)
    {
        try
        {
            var page = await _pageRepository.GetAsync(id);
            return ObjectMapper.Map<Page, PageDto>(page);
        }
        catch (UserFriendlyException)
        {
            // Already user-facing and taken care of by ABP framework, so we just rethrow
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error getting page id {id}");
            throw new UserFriendlyException("An unexpected error occurred while retrieving the page.", CmsPagesDomainErrorCodes.PageNotFound);
        }
    }

    public async Task<PagedResultDto<PageDto>> GetListAsync(PageFilterDto input)
    {
        var queryable = await _pageRepository.GetQueryableAsync();

        // Filtering
        queryable = queryable
            .WhereIf(!input.Title.IsNullOrWhiteSpace(), p => p.Title.Contains(input.Title!))
            .WhereIf(!input.RouteName.IsNullOrWhiteSpace(), p => p.RouteName.Contains(input.RouteName!));

        // Get total count after filtering
        var totalCount = await AsyncExecuter.CountAsync(queryable);

        // Apply sorting and pagination
        var query = queryable
            .OrderBy(input.Sorting.IsNullOrWhiteSpace() ? "Title" : input.Sorting)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var pages = await AsyncExecuter.ToListAsync(query);

        return new PagedResultDto<PageDto>(
            totalCount,
            ObjectMapper.Map<List<Page>, List<PageDto>>(pages)
        );
    }

    [UnitOfWork]
    private async Task UnsetOtherHomePageAsync(Guid? idToExclude = null)
    {
        var currentHomePage = await _pageRepository.FirstOrDefaultAsync(p => p.IsHomePage && p.Id != idToExclude);

        if (currentHomePage != null)
        {
            currentHomePage.IsHomePage = false;
            await _pageRepository.UpdateAsync(currentHomePage, autoSave: true);
        }
    }

    public string ConvertMarkdownToHtml(string markdownContent)
    {
        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        return Markdown.ToHtml(markdownContent, pipeline);
    }


    [Authorize(CmsPagesPermissions.Pages.Create)]
    public async Task<PageDto> CreateAsync(CreateUpdatePageDto input)
    {
        try
        {
            input.RouteName = _slugHelper.Slugify(input.RouteName);

            var pageWithSameRouteNameExists = await _pageRepository.AnyAsync(p => p.RouteName == input.RouteName);
            if (pageWithSameRouteNameExists)
            {
                throw new UserFriendlyException($"A page with the route name '{input.RouteName}' already exists.", CmsPagesDomainErrorCodes.PageCreationFailedExistingRoute);
            }

            if (input.IsHomePage)
            {
                await UnsetOtherHomePageAsync();
            }

            // Convert Markdown to HTML
            input.Content = ConvertMarkdownToHtml(input.Content);

            var page = ObjectMapper.Map<CreateUpdatePageDto, Page>(input);
            await _pageRepository.InsertAsync(page);
            return ObjectMapper.Map<Page, PageDto>(page);
        }
        catch (UserFriendlyException)
        {
            // Already user-facing and taken care of by ABP framework, so we just rethrow
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating page");
            throw new UserFriendlyException("An unexpected error occurred while creating the page.", CmsPagesDomainErrorCodes.PageCreationFailed);
        }
    }

    [Authorize(CmsPagesPermissions.Pages.Edit)]
    public async Task<PageDto> UpdateAsync(Guid id, CreateUpdatePageDto input)
    {
        try
        {
            input.RouteName = _slugHelper.Slugify(input.RouteName);

            var pageWithSameRouteNameExists = await _pageRepository.AnyAsync(p => p.RouteName == input.RouteName && p.Id != id);
            if (pageWithSameRouteNameExists)
            {
                throw new UserFriendlyException($"Another page already uses the route name '{input.RouteName}'.", CmsPagesDomainErrorCodes.PageUpdateFailedExistingRoute);
            }

            if (input.IsHomePage)
            {
                await UnsetOtherHomePageAsync(id);
            }


            var page = await _pageRepository.GetAsync(id);
            ObjectMapper.Map(input, page);
            await _pageRepository.UpdateAsync(page);
            return ObjectMapper.Map<Page, PageDto>(page);
        }
        catch (UserFriendlyException)
        {
            // Already user-facing and taken care of by ABP framework, so we just rethrow
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error updating page id {id}, '{input.Title}'");
            throw new UserFriendlyException("An unexpected error occurred while updating the page.", CmsPagesDomainErrorCodes.PageUpdateFailed);
        }
    }

    [Authorize(CmsPagesPermissions.Pages.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _pageRepository.DeleteAsync(id);
    }

    [AllowAnonymous]
    public async Task<List<PageMenuItemDto>> GetPageMenuItemsAsync()
    {
        var pages = await _pageRepository.GetListAsync();
        return pages.Select(page => new PageMenuItemDto
        {
            Name = $"Page_{page.Id}",
            DisplayName = page.Title,
            Url = $"/pages/{page.RouteName}"
        }).ToList();
    }

    [AllowAnonymous]
    public async Task<PageDto?> GetByRouteNameAsync(string routeName)
    {
        var page = await _pageRepository.FirstOrDefaultAsync(p => p.RouteName == routeName);
        if (page == null)
        {
            return null;
        }

        return ObjectMapper.Map<Page, PageDto>(page);
    }

    [AllowAnonymous]
    public async Task<PageDto?> GetHomePageAsync()
    {
        var page = await _pageRepository.FirstOrDefaultAsync(p => p.IsHomePage);
        if (page == null)
        {
            return null;
        }

        return ObjectMapper.Map<Page, PageDto>(page);
    }

    public string DecodeHtmlContent(string encodedContent)
    {
        return HttpUtility.HtmlDecode(encodedContent);
    }

    public string SanitizeHtml(string htmlContent)
    {
        var sanitizer = new HtmlSanitizer();
        return sanitizer.Sanitize(htmlContent);
    }

    public string GetDecodedAndSanitizedPageContentAsync(string encodedContent)
    {
        string decodedContent = DecodeHtmlContent(encodedContent);
        return SanitizeHtml(decodedContent);
    }
}