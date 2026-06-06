using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Sid.DbFieldManager.DbTables;

public class DbTableDto : FullAuditedEntityDto<Guid>
{
    public Guid? TargetDatabaseId { get; set; }
    public string TargetDatabaseName { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Schema { get; set; }
    public string Description { get; set; }
    public int FieldCount { get; set; }
}

public class DbTableLookupDto : EntityDto<Guid>
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
}

public class CreateDbTableDto
{
    public Guid? TargetDatabaseId { get; set; }

    [Required]
    [MaxLength(128)]
    public string Name { get; set; }

    [MaxLength(256)]
    public string DisplayName { get; set; }

    [MaxLength(128)]
    public string Schema { get; set; } = "dbo";

    [MaxLength(500)]
    public string Description { get; set; }
}

public class UpdateDbTableDto
{
    public Guid? TargetDatabaseId { get; set; }

    [Required]
    [MaxLength(128)]
    public string Name { get; set; }

    [MaxLength(256)]
    public string DisplayName { get; set; }

    [MaxLength(128)]
    public string Schema { get; set; } = "dbo";

    [MaxLength(500)]
    public string Description { get; set; }
}

public class DbTableGetListInput : PagedAndSortedResultRequestDto
{
    public Guid? TargetDatabaseId { get; set; }
    public string? Filter { get; set; }
}
