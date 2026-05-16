using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sid.DbFieldManager.TargetDatabases;

public class TargetDatabase : FullAuditedAggregateRoot<Guid>
{
    public TargetDatabase() { }
    public string Name { get; set; }
    public string ConnectionString { get; set; }
    public string Description { get; set; }

    public ICollection<DbTables.DbTable> Tables { get; set; }
}
