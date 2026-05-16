using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Sid.DbFieldManager.SqlExecutionLogs;

namespace Sid.DbFieldManager.SqlGeneration;

public class SqlExecutionAppService : ApplicationService, ISqlExecutionAppService
{
    private readonly ISqlGenerationAppService _sqlGenerationService;
    private readonly IRepository<DbFields.DbField, Guid> _fieldRepository;
    private readonly IRepository<TargetDatabases.TargetDatabase, Guid> _targetDbRepository;
    private readonly IRepository<SqlExecutionLog, Guid> _executionLogRepository;

    public SqlExecutionAppService(
        ISqlGenerationAppService sqlGenerationService,
        IRepository<DbFields.DbField, Guid> fieldRepository,
        IRepository<TargetDatabases.TargetDatabase, Guid> targetDbRepository,
        IRepository<SqlExecutionLog, Guid> executionLogRepository)
    {
        _sqlGenerationService = sqlGenerationService;
        _fieldRepository = fieldRepository;
        _targetDbRepository = targetDbRepository;
        _executionLogRepository = executionLogRepository;
    }

    public async Task<ExecuteSqlResult> ExecuteAsync(ExecuteSqlInput input)
    {
        var targetDb = await _targetDbRepository.GetAsync(input.TargetDatabaseId);
        var sqlResult = await _sqlGenerationService.PreviewAsync(new BatchGenerateSqlInput
        {
            FieldIds = input.FieldIds
        });

        var results = new List<FieldExecutionResult>();
        var allSuccess = true;

        using var connection = new SqlConnection(targetDb.ConnectionString);
        await connection.OpenAsync();

        foreach (var statement in sqlResult.Statements)
        {
            var fieldResult = new FieldExecutionResult
            {
                FieldId = statement.FieldId,
                FieldName = statement.FieldName
            };

            try
            {
                var field = await _fieldRepository.GetAsync(statement.FieldId);
                if (field.ExecutionStatus == DbFields.ExecutionStatus.Executed)
                {
                    fieldResult.Success = true;
                    fieldResult.Message = "已执行过，跳过";
                    results.Add(fieldResult);
                    continue;
                }

                using var transaction = connection.BeginTransaction();
                try
                {
                    using var cmd = connection.CreateCommand();
                    cmd.Transaction = transaction;
                    cmd.CommandText = statement.AlterTableSql;
                    await cmd.ExecuteNonQueryAsync();

                    if (!string.IsNullOrEmpty(statement.ExtendedPropertySql))
                    {
                        cmd.CommandText = statement.ExtendedPropertySql;
                        await cmd.ExecuteNonQueryAsync();
                    }

                    await transaction.CommitAsync();

                    field.ExecutionStatus = DbFields.ExecutionStatus.Executed;
                    field.ExecutedAt = DateTime.Now;
                    await _fieldRepository.UpdateAsync(field);

                    fieldResult.Success = true;
                    fieldResult.Message = "执行成功";

                    await LogExecution(targetDb.Id, statement.FieldId,
                        $"{statement.AlterTableSql}\n{statement.ExtendedPropertySql ?? ""}", true, null);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    field.ExecutionStatus = DbFields.ExecutionStatus.Failed;
                    await _fieldRepository.UpdateAsync(field);

                    fieldResult.Success = false;
                    fieldResult.Message = ex.Message;
                    allSuccess = false;

                    await LogExecution(targetDb.Id, statement.FieldId,
                        $"{statement.AlterTableSql}\n{statement.ExtendedPropertySql ?? ""}", false, ex.Message);
                }
            }
            catch (Exception ex)
            {
                fieldResult.Success = false;
                fieldResult.Message = ex.Message;
                allSuccess = false;
            }

            results.Add(fieldResult);
        }

        return new ExecuteSqlResult
        {
            AllSuccess = allSuccess,
            Results = results
        };
    }

    private async Task LogExecution(Guid targetDbId, Guid? fieldId, string sql, bool success, string error)
    {
        await _executionLogRepository.InsertAsync(new SqlExecutionLog
        {
            TargetDatabaseId = targetDbId,
            DbFieldId = fieldId,
            SqlScript = sql,
            IsSuccess = success,
            ErrorMessage = error ?? ""
        });
    }

    public async Task<TableExecuteSqlResult> ExecuteCreateTableAsync(ExecuteCreateTableInput input)
    {
        var targetDb = await _targetDbRepository.GetAsync(input.TargetDatabaseId);
        var sqlResult = await _sqlGenerationService.PreviewCreateTableAsync(new GenerateCreateTableSqlInput
        {
            TableId = input.TableId
        });

        try
        {
            using var connection = new SqlConnection(targetDb.ConnectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                using var cmd = connection.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandText = sqlResult.SqlScript;
                await cmd.ExecuteNonQueryAsync();
                await transaction.CommitAsync();

                await LogExecution(targetDb.Id, null, sqlResult.SqlScript, true, null);

                var query = await _fieldRepository.GetQueryableAsync();
                var fields = query.Where(f => f.DbTableId == input.TableId).ToList();
                foreach (var field in fields)
                {
                    field.ExecutionStatus = DbFields.ExecutionStatus.Executed;
                    field.ExecutedAt = DateTime.Now;
                    await _fieldRepository.UpdateAsync(field);
                }

                return new TableExecuteSqlResult
                {
                    Success = true,
                    Message = "建表成功"
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                await LogExecution(targetDb.Id, null, sqlResult.SqlScript, false, ex.Message);

                return new TableExecuteSqlResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        catch (Exception ex)
        {
            return new TableExecuteSqlResult
            {
                Success = false,
                Message = ex.Message
            };
        }
    }
}
