using Sid.DbFieldManager.Samples;
using Xunit;

namespace Sid.DbFieldManager.EntityFrameworkCore.Domains;

[Collection(DbFieldManagerTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<DbFieldManagerEntityFrameworkCoreTestModule>
{

}
