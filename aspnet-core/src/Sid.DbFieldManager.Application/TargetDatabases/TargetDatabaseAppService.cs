using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Sid.DbFieldManager.TargetDatabases;

public class TargetDatabaseAppService :
    CrudAppService<
        TargetDatabase,
        TargetDatabaseDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateTargetDatabaseDto,
        UpdateTargetDatabaseDto>,
    ITargetDatabaseAppService
{
    public TargetDatabaseAppService(IRepository<TargetDatabase, Guid> repository)
        : base(repository)
    {
    }

    public override async Task<TargetDatabaseDto> CreateAsync(CreateTargetDatabaseDto input)
    {
        var entity = new TargetDatabase
        {
            Name = input.Name,
            ConnectionString = input.ConnectionString,
            Description = input.Description
        };
        entity = await Repository.InsertAsync(entity, autoSave: true);
        return MapToGetOutputDto(entity);
    }

    public override async Task<TargetDatabaseDto> UpdateAsync(Guid id, UpdateTargetDatabaseDto input)
    {
        var entity = await Repository.GetAsync(id);
        entity.Name = input.Name;
        entity.ConnectionString = input.ConnectionString;
        entity.Description = input.Description;
        entity = await Repository.UpdateAsync(entity, autoSave: true);
        return MapToGetOutputDto(entity);
    }

    public override Task DeleteAsync(Guid id)
    {
        return base.DeleteAsync(id);
    }

    protected override TargetDatabaseDto MapToGetOutputDto(TargetDatabase entity)
    {
        return new TargetDatabaseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            ConnectionString = entity.ConnectionString,
            Description = entity.Description,
            TableCount = entity.Tables?.Count ?? 0,
            CreationTime = entity.CreationTime,
            CreatorId = entity.CreatorId,
            LastModificationTime = entity.LastModificationTime,
            LastModifierId = entity.LastModifierId,
            DeletionTime = entity.DeletionTime,
            DeleterId = entity.DeleterId,
            IsDeleted = entity.IsDeleted
        };
    }

    public async Task<ListResultDto<TargetDatabaseLookupDto>> GetLookupAsync()
    {
        var items = await Repository.GetListAsync();
        return new ListResultDto<TargetDatabaseLookupDto>(
            items.Select(x => new TargetDatabaseLookupDto { Id = x.Id, Name = x.Name }).ToList());
    }

    public async Task<bool> TestConnectionAsync(Guid id)
    {
        var target = await Repository.GetAsync(id);
        return await TestConnectionStringAsync(target.ConnectionString);
    }

    public Task<bool> TestConnectionStringAsync(string connectionString)
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}
