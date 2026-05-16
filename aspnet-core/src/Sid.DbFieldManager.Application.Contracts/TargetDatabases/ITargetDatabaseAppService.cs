using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Sid.DbFieldManager.TargetDatabases;

public interface ITargetDatabaseAppService :
    ICrudAppService<
        TargetDatabaseDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateTargetDatabaseDto,
        UpdateTargetDatabaseDto>
{
    Task<ListResultDto<TargetDatabaseLookupDto>> GetLookupAsync();
    Task<bool> TestConnectionAsync(Guid id);
    Task<bool> TestConnectionStringAsync(string connectionString);
}
