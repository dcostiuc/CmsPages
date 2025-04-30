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

namespace CmsPages.Pages;

[Authorize(CmsPagesPermissions.Pages.Default)]
public class PageAppService : ApplicationService, IPageAppService
{
    private readonly IRepository<Page, Guid> _repository;

    public PageAppService(IRepository<Page, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<PageDto> GetAsync(Guid id)
    {
        var page = await _repository.GetAsync(id);
        return ObjectMapper.Map<Page, PageDto>(page);
    }

    public async Task<PagedResultDto<PageDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var queryable = await _repository.GetQueryableAsync();
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
        await _repository.InsertAsync(page);
        return ObjectMapper.Map<Page, PageDto>(page);
    }

    [Authorize(CmsPagesPermissions.Pages.Edit)]
    public async Task<PageDto> UpdateAsync(Guid id, CreateUpdatePageDto input)
    {
        var page = await _repository.GetAsync(id);
        ObjectMapper.Map(input, page);
        await _repository.UpdateAsync(page);
        return ObjectMapper.Map<Page, PageDto>(page);
    }

    [Authorize(CmsPagesPermissions.Pages.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    [AllowAnonymous]
    public async Task<List<PageMenuItemDto>> GetPageMenuItemsAsync()
    {
        var pages = await _repository.GetListAsync();
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
        var page = await _repository.GetAsync(p => p.RouteName == routeName);

        return ObjectMapper.Map<Page, PageDto>(page);
    }


}
