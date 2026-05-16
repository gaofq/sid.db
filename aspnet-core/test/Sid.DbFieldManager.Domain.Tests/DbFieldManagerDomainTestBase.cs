using Volo.Abp.Modularity;

namespace Sid.DbFieldManager;

/* Inherit from this class for your domain layer tests. */
public abstract class DbFieldManagerDomainTestBase<TStartupModule> : DbFieldManagerTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
