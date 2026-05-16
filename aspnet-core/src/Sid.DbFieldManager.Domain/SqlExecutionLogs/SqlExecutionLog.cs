using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sid.DbFieldManager.SqlExecutionLogs;

public class SqlExecutionLog : CreationAuditedEntity<Guid>
{
    public Guid TargetDatabaseId { get; set; }
    public Guid? DbFieldId { get; set; }
    public string SqlScript { get; set; }
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }
}
