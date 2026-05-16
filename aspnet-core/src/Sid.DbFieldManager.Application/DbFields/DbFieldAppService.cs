using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Sid.DbFieldManager.DbFields;

public class DbFieldAppService :
    CrudAppService<
        DbField,
        DbFieldDto,
        Guid,
        DbFieldGetListInput,
        CreateDbFieldDto,
        UpdateDbFieldDto>,
    IDbFieldAppService
{
    public DbFieldAppService(IRepository<DbField, Guid> repository)
        : base(repository)
    {
    }

    protected override async Task<IQueryable<DbField>> CreateFilteredQueryAsync(DbFieldGetListInput input)
    {
        var query = await base.CreateFilteredQueryAsync(input);

        if (input.DbTableId.HasValue)
        {
            query = query.Where(x => x.DbTableId == input.DbTableId.Value);
        }

        if (!string.IsNullOrWhiteSpace(input.Filter))
        {
            query = query.Where(x =>
                x.Name.Contains(input.Filter) ||
                x.Description.Contains(input.Filter));
        }

        if (input.CreationTimeMin.HasValue)
        {
            query = query.Where(x => x.CreationTime >= input.CreationTimeMin.Value);
        }

        if (input.CreationTimeMax.HasValue)
        {
            query = query.Where(x => x.CreationTime < input.CreationTimeMax.Value.AddDays(1));
        }

        if (input.LastModificationTimeMin.HasValue)
        {
            query = query.Where(x => x.LastModificationTime >= input.LastModificationTimeMin.Value);
        }

        if (input.LastModificationTimeMax.HasValue)
        {
            query = query.Where(x => x.LastModificationTime < input.LastModificationTimeMax.Value.AddDays(1));
        }

        return query.OrderBy(x => x.SortOrder).ThenBy(x => x.CreationTime);
    }

    public override async Task<DbFieldDto> CreateAsync(CreateDbFieldDto input)
    {
        var entity = new DbField
        {
            DbTableId = input.DbTableId,
            Name = input.Name,
            SqlType = input.SqlType,
            IsNullable = input.IsNullable,
            MaxLength = input.MaxLength,
            DefaultValue = input.DefaultValue,
            Description = input.Description,
            SortOrder = input.SortOrder
        };
        entity = await Repository.InsertAsync(entity, autoSave: true);
        return MapToGetOutputDto(entity);
    }

    public override async Task<DbFieldDto> UpdateAsync(Guid id, UpdateDbFieldDto input)
    {
        var entity = await Repository.GetAsync(id);
        entity.Name = input.Name;
        entity.SqlType = input.SqlType;
        entity.IsNullable = input.IsNullable;
        entity.MaxLength = input.MaxLength;
        entity.DefaultValue = input.DefaultValue;
        entity.Description = input.Description;
        entity.SortOrder = input.SortOrder;
        entity = await Repository.UpdateAsync(entity, autoSave: true);
        return MapToGetOutputDto(entity);
    }

    public override Task DeleteAsync(Guid id)
    {
        return base.DeleteAsync(id);
    }

    protected override DbFieldDto MapToGetOutputDto(DbField entity)
    {
        return new DbFieldDto
        {
            Id = entity.Id,
            DbTableId = entity.DbTableId,
            Name = entity.Name,
            SqlType = entity.SqlType,
            IsNullable = entity.IsNullable,
            MaxLength = entity.MaxLength,
            DefaultValue = entity.DefaultValue,
            Description = entity.Description,
            SortOrder = entity.SortOrder,
            ExecutionStatus = entity.ExecutionStatus,
            ExecutedAt = entity.ExecutedAt,
            CreationTime = entity.CreationTime,
            CreatorId = entity.CreatorId,
            LastModificationTime = entity.LastModificationTime,
            LastModifierId = entity.LastModifierId,
            DeletionTime = entity.DeletionTime,
            DeleterId = entity.DeleterId,
            IsDeleted = entity.IsDeleted
        };
    }

    public async Task BatchCreateAsync(BatchCreateDbFieldDto input)
    {
        var query = await Repository.GetQueryableAsync();
        var existingFields = query.Where(x => x.DbTableId == input.DbTableId).ToList();
        var maxSort = existingFields.Any() ? existingFields.Max(x => x.SortOrder) : 0;

        var order = maxSort;
        foreach (var field in input.Fields)
        {
            var entity = new DbField
            {
                DbTableId = input.DbTableId,
                Name = field.Name,
                SqlType = field.SqlType,
                IsNullable = field.IsNullable,
                MaxLength = field.MaxLength,
                DefaultValue = field.DefaultValue,
                Description = field.Description,
                SortOrder = field.SortOrder > 0 ? field.SortOrder : ++order
            };
            await Repository.InsertAsync(entity);
        }
    }

    public async Task UpdateSortOrderAsync(Guid id, int sortOrder)
    {
        var field = await Repository.GetAsync(id);
        field.SortOrder = sortOrder;
        await Repository.UpdateAsync(field);
    }
}
