using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sid.DbFieldManager.DbFields;

public class DbField : FullAuditedEntity<Guid>
{
    public DbField() { }
    public Guid DbTableId { get; set; }
    public string Name { get; set; }
    public string SqlType { get; set; }
    public bool IsNullable { get; set; }
    public int? MaxLength { get; set; }
    public string DefaultValue { get; set; }
    public string Description { get; set; }
    public int SortOrder { get; set; }
    public ExecutionStatus ExecutionStatus { get; set; } = ExecutionStatus.Pending;
    public DateTime? ExecutedAt { get; set; }

    public DbTables.DbTable DbTable { get; set; }
}
