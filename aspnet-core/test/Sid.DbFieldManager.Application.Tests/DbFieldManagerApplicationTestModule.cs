using Volo.Abp.Modularity;

namespace Sid.DbFieldManager;

[DependsOn(
    typeof(DbFieldManagerApplicationModule),
    typeof(DbFieldManagerDomainTestModule)
)]
public class DbFieldManagerApplicationTestModule : AbpModule
{

}
