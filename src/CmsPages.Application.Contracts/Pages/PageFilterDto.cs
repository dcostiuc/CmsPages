using Volo.Abp.Application.Dtos;

namespace CmsPages.Pages;
public class PageFilterDto : PagedAndSortedResultRequestDto
{
    public string? Title { get; set; }
    public string? RouteName { get; set; }
}
