using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Sid.DbFieldManager.DbFields;

public interface IDbFieldAppService :
    ICrudAppService<
        DbFieldDto,
        Guid,
        DbFieldGetListInput,
        CreateDbFieldDto,
        UpdateDbFieldDto>
{
    Task BatchCreateAsync(BatchCreateDbFieldDto input);
    Task UpdateSortOrderAsync(Guid id, int sortOrder);
}
