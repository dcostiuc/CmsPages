using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace CmsPages.Pages;

public interface IPageAppService :
    ICrudAppService< //Defines CRUD methods
        PageDto, //Used to show pages
        Guid, //Primary key of the Page entity
        PagedAndSortedResultRequestDto, //Used for paging/sorting
        CreateUpdatePageDto> //Used to create/update a page
{

}