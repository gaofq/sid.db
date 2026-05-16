using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Sid.DbFieldManager.Data;

/* This is used if database provider does't define
 * IDbFieldManagerDbSchemaMigrator implementation.
 */
public class NullDbFieldManagerDbSchemaMigrator : IDbFieldManagerDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
