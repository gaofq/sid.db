using Xunit;

namespace Sid.DbFieldManager.EntityFrameworkCore;

[CollectionDefinition(DbFieldManagerTestConsts.CollectionDefinitionName)]
public class DbFieldManagerEntityFrameworkCoreCollection : ICollectionFixture<DbFieldManagerEntityFrameworkCoreFixture>
{

}
