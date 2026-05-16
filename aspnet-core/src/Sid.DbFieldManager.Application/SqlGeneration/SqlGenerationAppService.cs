using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Sid.DbFieldManager.SqlGeneration;

public class SqlGenerationAppService : ApplicationService, ISqlGenerationAppService
{
    private readonly IRepository<DbFields.DbField, Guid> _fieldRepository;
    private readonly IRepository<DbTables.DbTable, Guid> _tableRepository;

    public SqlGenerationAppService(
        IRepository<DbFields.DbField, Guid> fieldRepository,
        IRepository<DbTables.DbTable, Guid> tableRepository)
    {
        _fieldRepository = fieldRepository;
        _tableRepository = tableRepository;
    }

    public async Task<GeneratedSqlResult> PreviewCreateTableAsync(GenerateCreateTableSqlInput input)
    {
        var table = await _tableRepository.GetAsync(input.TableId);
        var tableName = table.Name;
        var schema = table.Schema ?? "dbo";
        var displayName = table.DisplayName ?? "";

        var query = await _fieldRepository.GetQueryableAsync();
        var fields = query
            .Where(f => f.DbTableId == input.TableId)
            .OrderBy(f => f.SortOrder)
            .ToList();

        if (fields.Count == 0)
        {
            throw new UserFriendlyException("该表没有字段，请先添加字段");
        }

        var sb = new StringBuilder();
        var statements = new List<GeneratedSqlStatement>();

        sb.AppendLine("-- ==========================================");
        sb.AppendLine($"-- Create Table: [{schema}].[{tableName}]");
        sb.AppendLine($"-- Description: {displayName}");
        sb.AppendLine($"-- Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine("-- ==========================================");
        sb.AppendLine();

        var fullTableName = $"[{schema}].[{tableName}]";
        var columnDefs = new List<string>();
        var extPropSqls = new List<string>();

        foreach (var field in fields)
        {
            var nullClause = field.IsNullable ? " NULL" : " NOT NULL";
            var defaultClause = "";
            if (!string.IsNullOrWhiteSpace(field.DefaultValue))
            {
                defaultClause = $" DEFAULT {field.DefaultValue}";
            }

            var columnDef = $"    [{field.Name}] {field.SqlType}{nullClause}{defaultClause}";
            columnDefs.Add(columnDef);

            if (!string.IsNullOrWhiteSpace(field.Description))
            {
                var descEscaped = field.Description.Replace("'", "''");
                extPropSqls.Add($"EXEC sp_addextendedproperty N'MS_Description', N'{descEscaped}', N'user', N'dbo', N'table', N'{tableName}', N'column', N'{field.Name}'");
            }
        }

        sb.AppendLine($"IF OBJECT_ID('{fullTableName}', 'U') IS NOT NULL");
        sb.AppendLine($"    DROP TABLE {fullTableName};");
        sb.AppendLine();
        sb.AppendLine($"CREATE TABLE {fullTableName} (");
        sb.AppendLine(string.Join(",\n", columnDefs));
        sb.AppendLine(");");
        sb.AppendLine();

        if (extPropSqls.Count > 0)
        {
            sb.AppendLine("-- Column descriptions");
            foreach (var sql in extPropSqls)
            {
                sb.AppendLine(sql);
            }
            sb.AppendLine();
        }

        return new GeneratedSqlResult
        {
            SqlScript = sb.ToString(),
            TableName = tableName,
            Statements = statements
        };
    }

    public async Task<GeneratedSqlResult> PreviewAsync(BatchGenerateSqlInput input)
    {
        if (input.FieldIds == null || input.FieldIds.Count == 0)
        {
            throw new UserFriendlyException("请至少选择一个字段");
        }

        var query = await _fieldRepository.GetQueryableAsync();
        var selectedFields = query
            .Where(f => input.FieldIds.Contains(f.Id))
            .OrderBy(f => f.SortOrder)
            .ToList();

        if (selectedFields.Count == 0)
        {
            throw new UserFriendlyException("未找到所选字段");
        }

        var tableId = selectedFields.First().DbTableId;
        var table = await _tableRepository.GetAsync(tableId);
        var tableName = table.Name;
        var schema = table.Schema ?? "dbo";
        var displayName = table.DisplayName ?? "";

        var sb = new StringBuilder();
        var statements = new List<GeneratedSqlStatement>();

        sb.AppendLine("-- ==========================================");
        sb.AppendLine($"-- Table: [{schema}].[{tableName}]");
        sb.AppendLine($"-- Description: {displayName}");
        sb.AppendLine($"-- Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine("-- ==========================================");
        sb.AppendLine();

        foreach (var field in selectedFields)
        {
            var nullClause = field.IsNullable ? " NULL" : " NOT NULL";
            var defaultClause = "";
            if (!string.IsNullOrWhiteSpace(field.DefaultValue))
            {
                defaultClause = $" DEFAULT {field.DefaultValue}";
            }

            var alterSql = $"ALTER TABLE [{schema}].[{tableName}] ADD [{field.Name}] {field.SqlType}{nullClause}{defaultClause}";

            string extPropSql = null;
            if (!string.IsNullOrWhiteSpace(field.Description))
            {
                var descEscaped = field.Description.Replace("'", "''");
                extPropSql = $"EXEC sp_addextendedproperty N'MS_Description', N'{descEscaped}', N'user', N'dbo', N'table', N'{tableName}', N'column', N'{field.Name}'";
            }

            sb.AppendLine(alterSql);
            statements.Add(new GeneratedSqlStatement
            {
                FieldId = field.Id,
                FieldName = field.Name,
                AlterTableSql = alterSql,
                ExtendedPropertySql = extPropSql
            });

            if (extPropSql != null)
            {
                sb.AppendLine(extPropSql);
            }

            sb.AppendLine("GO");
            sb.AppendLine();
        }

        return new GeneratedSqlResult
        {
            SqlScript = sb.ToString(),
            TableName = tableName,
            Statements = statements
        };
    }
}
