using CmsPages.Pages;
using Xunit;

namespace CmsPages.EntityFrameworkCore.Applications.Pages;

[Collection(CmsPagesTestConsts.CollectionDefinitionName)]
public class EfCorePageAppService_Tests : PageAppService_Tests<CmsPagesEntityFrameworkCoreTestModule>
{

}