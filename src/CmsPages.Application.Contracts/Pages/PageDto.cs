using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace CmsPages.Pages;

public class PageDto : FullAuditedEntityDto<Guid>
{
    [StringLength(PageConsts.MaxTitleLength)]
    public string Title { get; set; }

    [StringLength(PageConsts.MaxRouteNameLength)]
    public string RouteName { get; set; }

    [StringLength(PageConsts.MaxContentLength, ErrorMessage = "Content is too long.")]
    public string? Content { get; set; }

    public bool IsHomePage { get; set; }
}