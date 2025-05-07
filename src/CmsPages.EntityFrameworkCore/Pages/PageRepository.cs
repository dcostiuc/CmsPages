using System;
using CmsPages.EntityFrameworkCore;
using CmsPages.Pages;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using CmsPages.Pages;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.EntityFrameworkCore;

public class PageRepository : EfCoreRepository<CmsPagesDbContext, Page, Guid>, IPageRepository
{
    public PageRepository(IDbContextProvider<CmsPagesDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<List<Page>> GetListAsync(int skipCount, int maxResultCount, string sorting, PageFilterDto filter)
    {
        var query = await GetQueryableAsync();

        query = query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.Title), x => x.Title.Contains(filter.Title))
            .WhereIf(!string.IsNullOrWhiteSpace(filter.RouteName), x => x.RouteName.Contains(filter.RouteName));

        query = string.IsNullOrWhiteSpace(sorting)
            ? query.OrderBy(x => x.Title)
            : query.OrderBy(e => EF.Property<object>(e, sorting));

        return await query
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync();
    }

    public async Task<int> GetCountAsync(PageFilterDto filter)
    {
        var query = await GetQueryableAsync();

        query = query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.Title), x => x.Title.Contains(filter.Title))
            .WhereIf(!string.IsNullOrWhiteSpace(filter.RouteName), x => x.RouteName.Contains(filter.RouteName));

        return await query.CountAsync();
    }
}
