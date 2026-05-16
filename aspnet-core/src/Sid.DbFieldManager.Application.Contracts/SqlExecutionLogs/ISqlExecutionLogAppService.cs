using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Sid.DbFieldManager.SqlExecutionLogs;

public interface ISqlExecutionLogAppService :
    IReadOnlyAppService<
        SqlExecutionLogDto,
        Guid,
        SqlExecutionLogGetListInput>
{
}
