using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Sid.DbFieldManager.DbTables;

public class DbTableAppService :
    CrudAppService<
        DbTable,
        DbTableDto,
        Guid,
        DbTableGetListInput,
        CreateDbTableDto,
        UpdateDbTableDto>,
    IDbTableAppService
{
    public DbTableAppService(IRepository<DbTable, Guid> repository)
        : base(repository)
    {
    }

    protected override async Task<IQueryable<DbTable>> CreateFilteredQueryAsync(DbTableGetListInput input)
    {
        var query = await base.CreateFilteredQueryAsync(input);

        if (!string.IsNullOrWhiteSpace(input.Filter))
        {
            query = query.Where(x =>
                x.Name.Contains(input.Filter) ||
                x.DisplayName.Contains(input.Filter));
        }

        return query;
    }

    public override async Task<DbTableDto> CreateAsync(CreateDbTableDto input)
    {
        var entity = new DbTable
        {
            Name = input.Name,
            DisplayName = input.DisplayName,
            Schema = input.Schema,
            Description = input.Description
        };
        entity = await Repository.InsertAsync(entity, autoSave: true);
        return await MapToGetOutputDtoAsync(entity);
    }

    public override async Task<DbTableDto> UpdateAsync(Guid id, UpdateDbTableDto input)
    {
        var entity = await Repository.GetAsync(id);
        entity.Name = input.Name;
        entity.DisplayName = input.DisplayName;
        entity.Schema = input.Schema;
        entity.Description = input.Description;
        entity = await Repository.UpdateAsync(entity, autoSave: true);
        return await MapToGetOutputDtoAsync(entity);
    }

    public override Task DeleteAsync(Guid id)
    {
        return base.DeleteAsync(id);
    }

    protected override async Task<DbTableDto> MapToGetOutputDtoAsync(DbTable entity)
    {
        return new DbTableDto
        {
            Id = entity.Id,
            Name = entity.Name,
            DisplayName = entity.DisplayName,
            Schema = entity.Schema,
            Description = entity.Description,
            FieldCount = entity.Fields?.Count ?? 0,
            CreationTime = entity.CreationTime,
            CreatorId = entity.CreatorId,
            LastModificationTime = entity.LastModificationTime,
            LastModifierId = entity.LastModifierId,
            DeletionTime = entity.DeletionTime,
            DeleterId = entity.DeleterId,
            IsDeleted = entity.IsDeleted
        };
    }

    public async Task<ListResultDto<DbTableLookupDto>> GetLookupAsync()
    {
        var query = await Repository.GetQueryableAsync();
        var items = query.ToList();
        return new ListResultDto<DbTableLookupDto>(
            items.Select(x => new DbTableLookupDto { Id = x.Id, Name = x.Name, DisplayName = x.DisplayName }).ToList());
    }
}
