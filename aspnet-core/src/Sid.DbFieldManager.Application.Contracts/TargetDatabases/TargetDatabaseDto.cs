using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Sid.DbFieldManager.TargetDatabases;

public class TargetDatabaseDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; }
    public string ConnectionString { get; set; }
    public string Description { get; set; }
    public int TableCount { get; set; }
}

public class TargetDatabaseLookupDto : EntityDto<Guid>
{
    public string Name { get; set; }
}

public class CreateTargetDatabaseDto
{
    [Required]
    [MaxLength(128)]
    public string Name { get; set; }

    [Required]
    [MaxLength(500)]
    public string ConnectionString { get; set; }

    [MaxLength(256)]
    public string Description { get; set; }
}

public class UpdateTargetDatabaseDto
{
    [Required]
    [MaxLength(128)]
    public string Name { get; set; }

    [Required]
    [MaxLength(500)]
    public string ConnectionString { get; set; }

    [MaxLength(256)]
    public string Description { get; set; }
}
