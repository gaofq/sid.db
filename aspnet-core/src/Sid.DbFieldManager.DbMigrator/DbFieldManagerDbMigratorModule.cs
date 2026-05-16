using Sid.DbFieldManager.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Sid.DbFieldManager.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(DbFieldManagerEntityFrameworkCoreModule),
    typeof(DbFieldManagerApplicationContractsModule)
    )]
public class DbFieldManagerDbMigratorModule : AbpModule
{
}
