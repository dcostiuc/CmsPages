using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CmsPages.Pages;
using Volo.Abp.Domain.Repositories;

public interface IPageRepository : IRepository<Page, Guid>
{
    Task<List<Page>> GetListAsync(int skipCount, int maxResultCount, string sorting, PageFilterDto filter);
    Task<int> GetCountAsync(PageFilterDto filter);
}
