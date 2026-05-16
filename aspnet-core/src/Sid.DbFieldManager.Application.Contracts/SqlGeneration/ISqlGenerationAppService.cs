using System.Threading.Tasks;

namespace Sid.DbFieldManager.SqlGeneration;

public interface ISqlGenerationAppService
{
    Task<GeneratedSqlResult> PreviewAsync(BatchGenerateSqlInput input);
    Task<GeneratedSqlResult> PreviewCreateTableAsync(GenerateCreateTableSqlInput input);
}

public interface ISqlExecutionAppService
{
    Task<ExecuteSqlResult> ExecuteAsync(ExecuteSqlInput input);
    Task<TableExecuteSqlResult> ExecuteCreateTableAsync(ExecuteCreateTableInput input);
}
