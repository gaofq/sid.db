using Riok.Mapperly.Abstractions;
using Sid.DbFieldManager.DbFields;
using Sid.DbFieldManager.DbTables;
using Sid.DbFieldManager.SqlExecutionLogs;
using Sid.DbFieldManager.TargetDatabases;
using Volo.Abp.Mapperly;

namespace Sid.DbFieldManager;

[Mapper]
public partial class DbFieldManagerApplicationMappers
{
    public partial TargetDatabaseDto Map(TargetDatabase entity);

    public partial DbTableDto Map(DbTable entity);

    public partial DbFieldDto Map(DbField entity);

    public partial SqlExecutionLogDto Map(SqlExecutionLog entity);
}
