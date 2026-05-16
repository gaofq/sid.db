using System;
using System.Collections.Generic;

namespace Sid.DbFieldManager.SqlGeneration;

public class BatchGenerateSqlInput
{
    public List<Guid> FieldIds { get; set; }
}

public class GeneratedSqlResult
{
    public string SqlScript { get; set; }
    public string TableName { get; set; }
    public List<GeneratedSqlStatement> Statements { get; set; }
}

public class GeneratedSqlStatement
{
    public Guid FieldId { get; set; }
    public string FieldName { get; set; }
    public string AlterTableSql { get; set; }
    public string ExtendedPropertySql { get; set; }
}

public class ExecuteSqlInput
{
    public Guid TargetDatabaseId { get; set; }
    public List<Guid> FieldIds { get; set; }
}

public class ExecuteSqlResult
{
    public bool AllSuccess { get; set; }
    public List<FieldExecutionResult> Results { get; set; }
}

public class FieldExecutionResult
{
    public Guid FieldId { get; set; }
    public string FieldName { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
}

public class GenerateCreateTableSqlInput
{
    public Guid TableId { get; set; }
}

public class ExecuteCreateTableInput
{
    public Guid TableId { get; set; }
    public Guid TargetDatabaseId { get; set; }
}

public class TableExecuteSqlResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
}
