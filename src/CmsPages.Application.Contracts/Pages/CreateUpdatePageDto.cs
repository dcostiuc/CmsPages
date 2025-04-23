using System.ComponentModel.DataAnnotations;

namespace CmsPages.Pages;

public class CreateUpdatePageDto
{
    [Required]
    [StringLength(128)]
    public string Title { get; set; }


    [Required]
    [StringLength(128)]
    public string RouteName { get; set; }

    public string? Content { get; set; }

    public bool IsHomePage { get; set; }
}