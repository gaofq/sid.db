using System.Threading.Tasks;

namespace Sid.DbFieldManager.Data;

public interface IDbFieldManagerDbSchemaMigrator
{
    Task MigrateAsync();
}
