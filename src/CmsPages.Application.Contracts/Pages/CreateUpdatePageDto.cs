using System.ComponentModel.DataAnnotations;

namespace CmsPages.Pages;

public class CreateUpdatePageDto
{
    [Required]
    [StringLength(PageConsts.MaxTitleLength)]
    public string Title { get; set; }


    [Required]
    [StringLength(PageConsts.MaxRouteNameLength)]
    public string RouteName { get; set; }

    [StringLength(PageConsts.MaxContentLength, ErrorMessage = "Content is too long.")]
    public string? Content { get; set; }

    public bool IsHomePage { get; set; }
}