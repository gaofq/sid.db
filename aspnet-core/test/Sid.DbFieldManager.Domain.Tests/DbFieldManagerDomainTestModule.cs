using Volo.Abp.Modularity;

namespace Sid.DbFieldManager;

[DependsOn(
    typeof(DbFieldManagerDomainModule),
    typeof(DbFieldManagerTestBaseModule)
)]
public class DbFieldManagerDomainTestModule : AbpModule
{

}
