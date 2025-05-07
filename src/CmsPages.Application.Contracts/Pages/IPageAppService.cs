using System;
using System.Collections.Generic; // For List<>
using System.Threading.Tasks; // For Task<>
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
namespace CmsPages.Pages;

public interface IPageAppService :
    ICrudAppService< //Defines CRUD methods
        PageDto, //Used to show pages
        Guid, //Primary key of the Page entity
        PageFilterDto, //Used for paging/sorting/filtering
        CreateUpdatePageDto> //Used to create/update a page
{
    Task<List<PageMenuItemDto>> GetPageMenuItemsAsync();
    Task<PageDto?> GetByRouteNameAsync(string routeName);
    Task<PageDto?> GetHomePageAsync();
    public string DecodeHtmlContent(string encodedContent);
    public string SanitizeHtml(string htmlContent);
    public string GetDecodedAndSanitizedPageContentAsync(string encodedContent);
    public string ConvertMarkdownToHtml(string markdownContent);
}