using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace CmsPages.Pages;

public class Page : AuditedAggregateRoot<Guid>
{
    public required string Title { get; set; }

    public required string RouteName { get; set; }

    public string? Content { get; set; }

    public bool IsHomePage { get; set; }
}