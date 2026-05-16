using Volo.Abp.Modularity;

namespace Sid.DbFieldManager;

public abstract class DbFieldManagerApplicationTestBase<TStartupModule> : DbFieldManagerTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
