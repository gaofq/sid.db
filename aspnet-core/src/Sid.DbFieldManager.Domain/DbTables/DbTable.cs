using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sid.DbFieldManager.DbTables;

public class DbTable : FullAuditedAggregateRoot<Guid>
{
    public DbTable() { }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Schema { get; set; } = "dbo";
    public string Description { get; set; }
}
