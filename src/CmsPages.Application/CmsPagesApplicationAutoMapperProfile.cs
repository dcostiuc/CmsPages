using AutoMapper;
using CmsPages.Pages;

namespace CmsPages;

public class CmsPagesApplicationAutoMapperProfile : Profile
{
    public CmsPagesApplicationAutoMapperProfile()
    {
        CreateMap<Pages.Page, PageDto>();
        CreateMap<CreateUpdatePageDto, Pages.Page>();
        CreateMap<Pages.Page, CreateUpdatePageDto>();
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
    }
}
