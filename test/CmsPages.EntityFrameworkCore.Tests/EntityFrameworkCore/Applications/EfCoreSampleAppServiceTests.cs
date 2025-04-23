using CmsPages.Samples;
using Xunit;

namespace CmsPages.EntityFrameworkCore.Applications;

[Collection(CmsPagesTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<CmsPagesEntityFrameworkCoreTestModule>
{

}
