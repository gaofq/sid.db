using Sid.DbFieldManager.Samples;
using Xunit;

namespace Sid.DbFieldManager.EntityFrameworkCore.Applications;

[Collection(DbFieldManagerTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<DbFieldManagerEntityFrameworkCoreTestModule>
{

}
