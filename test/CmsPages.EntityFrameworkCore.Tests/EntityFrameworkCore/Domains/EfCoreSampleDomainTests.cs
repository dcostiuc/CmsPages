using CmsPages.Samples;
using Xunit;

namespace CmsPages.EntityFrameworkCore.Domains;

[Collection(CmsPagesTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<CmsPagesEntityFrameworkCoreTestModule>
{

}
