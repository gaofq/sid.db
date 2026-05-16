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
    private readonly IRepository<TargetDatabases.TargetDatabase, Guid> _targetDbRepo;

    public DbTableAppService(
        IRepository<DbTable, Guid> repository,
        IRepository<TargetDatabases.TargetDatabase, Guid> targetDbRepo)
        : base(repository)
    {
        _targetDbRepo = targetDbRepo;
    }

    protected override async Task<IQueryable<DbTable>> CreateFilteredQueryAsync(DbTableGetListInput input)
    {
        var query = await base.CreateFilteredQueryAsync(input);

        if (input.TargetDatabaseId.HasValue)
        {
            query = query.Where(x => x.TargetDatabaseId == input.TargetDatabaseId.Value);
        }

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
            TargetDatabaseId = input.TargetDatabaseId,
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
        var dto = new DbTableDto
        {
            Id = entity.Id,
            TargetDatabaseId = entity.TargetDatabaseId,
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

        if (entity.TargetDatabase != null)
        {
            dto.TargetDatabaseName = entity.TargetDatabase.Name;
        }
        else
        {
            var targetDb = await _targetDbRepo.FindAsync(entity.TargetDatabaseId);
            dto.TargetDatabaseName = targetDb?.Name;
        }

        return dto;
    }

    public async Task<ListResultDto<DbTableLookupDto>> GetLookupAsync(Guid? targetDatabaseId = null)
    {
        var query = await Repository.GetQueryableAsync();
        if (targetDatabaseId.HasValue)
        {
            query = query.Where(x => x.TargetDatabaseId == targetDatabaseId.Value);
        }
        var items = query.ToList();
        return new ListResultDto<DbTableLookupDto>(
            items.Select(x => new DbTableLookupDto { Id = x.Id, Name = x.Name, DisplayName = x.DisplayName }).ToList());
    }
}
