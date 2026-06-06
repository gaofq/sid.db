using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Sid.DbFieldManager.DbTables;

public interface IDbTableAppService :
    ICrudAppService<
        DbTableDto,
        Guid,
        DbTableGetListInput,
        CreateDbTableDto,
        UpdateDbTableDto>
{
    Task<ListResultDto<DbTableLookupDto>> GetLookupAsync();
}
