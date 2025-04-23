using System;
using Volo.Abp.Application.Dtos;

namespace CmsPages.Pages;

public class PageDto : AuditedEntityDto<Guid>
{
    public string Title { get; set; }

    public string RouteName { get; set; }

    public string? Content { get; set; }

    public bool IsHomePage { get; set; }
}