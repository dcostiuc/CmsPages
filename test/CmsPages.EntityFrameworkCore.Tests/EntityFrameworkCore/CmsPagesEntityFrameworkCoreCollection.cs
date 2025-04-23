using Xunit;

namespace CmsPages.EntityFrameworkCore;

[CollectionDefinition(CmsPagesTestConsts.CollectionDefinitionName)]
public class CmsPagesEntityFrameworkCoreCollection : ICollectionFixture<CmsPagesEntityFrameworkCoreFixture>
{

}
