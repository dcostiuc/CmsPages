using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace CmsPages.Pages;

public class Page : FullAuditedAggregateRoot<Guid>
{
    [StringLength(PageConsts.MaxTitleLength)]
    public required string Title { get; set; }

    [StringLength(PageConsts.MaxRouteNameLength)]
    public required string RouteName { get; set; }

    [StringLength(PageConsts.MaxContentLength)]
    public string? Content { get; set; }

    public bool IsHomePage { get; set; }
}