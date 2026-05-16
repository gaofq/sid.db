using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Sid.DbFieldManager.DbFields;

public class DbFieldDto : FullAuditedEntityDto<Guid>
{
    public Guid DbTableId { get; set; }
    public string TableName { get; set; }
    public string Name { get; set; }
    public string SqlType { get; set; }
    public bool IsNullable { get; set; }
    public int? MaxLength { get; set; }
    public string DefaultValue { get; set; }
    public string Description { get; set; }
    public int SortOrder { get; set; }
    public ExecutionStatus ExecutionStatus { get; set; }
    public DateTime? ExecutedAt { get; set; }
}

public class CreateDbFieldDto
{
    [Required]
    public Guid DbTableId { get; set; }

    [Required]
    [MaxLength(128)]
    public string Name { get; set; }

    [Required]
    [MaxLength(50)]
    public string SqlType { get; set; }

    public bool IsNullable { get; set; }
    public int? MaxLength { get; set; }

    [MaxLength(200)]
    public string DefaultValue { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }

    public int SortOrder { get; set; }
}

public class UpdateDbFieldDto
{
    [Required]
    [MaxLength(128)]
    public string Name { get; set; }

    [Required]
    [MaxLength(50)]
    public string SqlType { get; set; }

    public bool IsNullable { get; set; }
    public int? MaxLength { get; set; }

    [MaxLength(200)]
    public string DefaultValue { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }

    public int SortOrder { get; set; }
}

public class BatchCreateDbFieldDto
{
    [Required]
    public Guid DbTableId { get; set; }

    [Required]
    public List<CreateDbFieldDto> Fields { get; set; }
}

public class DbFieldGetListInput : PagedAndSortedResultRequestDto
{
    public Guid? DbTableId { get; set; }
    public string? Filter { get; set; }
    public DateTime? CreationTimeMin { get; set; }
    public DateTime? CreationTimeMax { get; set; }
    public DateTime? LastModificationTimeMin { get; set; }
    public DateTime? LastModificationTimeMax { get; set; }
}
