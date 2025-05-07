using AutoMapper;
using CmsPages.Pages;

namespace CmsPages.Blazor;

public class CmsPagesBlazorAutoMapperProfile : Profile
{
    public CmsPagesBlazorAutoMapperProfile()
    {
        CreateMap<PageDto, CreateUpdatePageDto>();
        CreateMap<Page, PageDto>();
    }
}
