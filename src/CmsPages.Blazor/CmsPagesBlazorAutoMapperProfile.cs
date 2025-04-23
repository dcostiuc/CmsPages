using AutoMapper;
using CmsPages.Pages;

namespace CmsPages.Blazor;

public class CmsPagesBlazorAutoMapperProfile : Profile
{
    public CmsPagesBlazorAutoMapperProfile()
    {
        CreateMap<PageDto, CreateUpdatePageDto>();

        //Define your AutoMapper configuration here for the Blazor project.
    }
}
