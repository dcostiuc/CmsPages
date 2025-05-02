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

namespace CmsPages.Pages;

[Authorize(CmsPagesPermissions.Pages.Default)]
public class PageAppService : ApplicationService, IPageAppService
{
    private readonly IRepository<Page, Guid> _pageRepository;

    public PageAppService(IRepository<Page, Guid> repository)
    {
        _pageRepository = repository;
    }

    public async Task<PageDto> GetAsync(Guid id)
    {
        var page = await _pageRepository.GetAsync(id);
        return ObjectMapper.Map<Page, PageDto>(page);
    }

    public async Task<PagedResultDto<PageDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var queryable = await _pageRepository.GetQueryableAsync();
        var query = queryable
            .OrderBy(input.Sorting.IsNullOrWhiteSpace() ? "Title" : input.Sorting)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var pages = await AsyncExecuter.ToListAsync(query);
        var totalCount = await AsyncExecuter.CountAsync(queryable);

        return new PagedResultDto<PageDto>(
            totalCount,
            ObjectMapper.Map<List<Page>, List<PageDto>>(pages)
        );
    }

    [Authorize(CmsPagesPermissions.Pages.Create)]
    public async Task<PageDto> CreateAsync(CreateUpdatePageDto input)
    {
        var page = ObjectMapper.Map<CreateUpdatePageDto, Page>(input);
        await _pageRepository.InsertAsync(page);
        return ObjectMapper.Map<Page, PageDto>(page);
    }

    [Authorize(CmsPagesPermissions.Pages.Edit)]
    public async Task<PageDto> UpdateAsync(Guid id, CreateUpdatePageDto input)
    {
        var page = await _pageRepository.GetAsync(id);
        ObjectMapper.Map(input, page);
        await _pageRepository.UpdateAsync(page);
        return ObjectMapper.Map<Page, PageDto>(page);
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
    public async Task<PageDto> GetByRouteNameAsync(string routeName)
    {
        var page = await _pageRepository.FirstOrDefaultAsync(p => p.RouteName == routeName);
        if (page == null)
        {
            return null;
        }

        return ObjectMapper.Map<Page, PageDto>(page);
    }

    public async Task<PageDto> GetHomePageAsync()
    {
        var page = await _pageRepository.FirstOrDefaultAsync(p => p.IsHomePage);
        if (page == null)
        {
            return null;
        }

        return new PageDto
        {
            Id = page.Id,
            Title = page.Title,
            Content = page.Content,
            RouteName = page.RouteName
        };
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