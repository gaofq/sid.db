using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Sid.DbFieldManager.SqlExecutionLogs;

public class SqlExecutionLogDto : EntityDto<Guid>
{
    public Guid TargetDatabaseId { get; set; }
    public string TargetDatabaseName { get; set; }
    public Guid? DbFieldId { get; set; }
    public string DbFieldName { get; set; }
    public string SqlScript { get; set; }
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }
    public DateTime ExecutedAt { get; set; }
    public Guid? ExecutedBy { get; set; }
}

public class SqlExecutionLogGetListInput : PagedAndSortedResultRequestDto
{
    public Guid? TargetDatabaseId { get; set; }
    public Guid? DbTableId { get; set; }
}
