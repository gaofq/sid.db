using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sid.DbFieldManager.Data;
using Volo.Abp.DependencyInjection;

namespace Sid.DbFieldManager.EntityFrameworkCore;

public class EntityFrameworkCoreDbFieldManagerDbSchemaMigrator
    : IDbFieldManagerDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreDbFieldManagerDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the DbFieldManagerDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<DbFieldManagerDbContext>()
            .Database
            .MigrateAsync();
    }
}
