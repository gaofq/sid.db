using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Sid.DbFieldManager.SqlExecutionLogs;

public class SqlExecutionLogAppService :
    ReadOnlyAppService<
        SqlExecutionLog,
        SqlExecutionLogDto,
        Guid,
        SqlExecutionLogGetListInput>,
    ISqlExecutionLogAppService
{
    private readonly IRepository<TargetDatabases.TargetDatabase, Guid> _targetDbRepo;
    private readonly IRepository<DbFields.DbField, Guid> _fieldRepo;

    public SqlExecutionLogAppService(
        IRepository<SqlExecutionLog, Guid> repository,
        IRepository<TargetDatabases.TargetDatabase, Guid> targetDbRepo,
        IRepository<DbFields.DbField, Guid> fieldRepo)
        : base(repository)
    {
        _targetDbRepo = targetDbRepo;
        _fieldRepo = fieldRepo;
    }

    protected override async Task<IQueryable<SqlExecutionLog>> CreateFilteredQueryAsync(SqlExecutionLogGetListInput input)
    {
        var query = await base.CreateFilteredQueryAsync(input);

        if (input.TargetDatabaseId.HasValue)
        {
            query = query.Where(x => x.TargetDatabaseId == input.TargetDatabaseId.Value);
        }

        return query.OrderByDescending(x => x.CreationTime);
    }

    protected override async Task<List<SqlExecutionLogDto>> MapToGetListOutputDtosAsync(List<SqlExecutionLog> entities)
    {
        var targetDbIds = entities.Select(e => e.TargetDatabaseId).Distinct().ToList();
        var fieldIds = entities.Where(e => e.DbFieldId.HasValue).Select(e => e.DbFieldId!.Value).Distinct().ToList();

        var targetDbs = new Dictionary<Guid, string>();
        var fieldNames = new Dictionary<Guid, string>();

        foreach (var id in targetDbIds)
        {
            var db = await _targetDbRepo.FindAsync(id);
            targetDbs[id] = db?.Name;
        }

        foreach (var id in fieldIds)
        {
            var field = await _fieldRepo.FindAsync(id);
            fieldNames[id] = field?.Name;
        }

        return entities.Select(e => new SqlExecutionLogDto
        {
            Id = e.Id,
            TargetDatabaseId = e.TargetDatabaseId,
            TargetDatabaseName = targetDbs.GetValueOrDefault(e.TargetDatabaseId),
            DbFieldId = e.DbFieldId,
            DbFieldName = e.DbFieldId.HasValue ? fieldNames.GetValueOrDefault(e.DbFieldId.Value) : null,
            SqlScript = e.SqlScript,
            IsSuccess = e.IsSuccess,
            ErrorMessage = e.ErrorMessage,
            ExecutedAt = e.CreationTime,
            ExecutedBy = e.CreatorId
        }).ToList();
    }
}
