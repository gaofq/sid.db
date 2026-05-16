using System;
using System.Collections.Generic;
using System.Text;
using Sid.DbFieldManager.Localization;
using Volo.Abp.Application.Services;

namespace Sid.DbFieldManager;

/* Inherit your application services from this class.
 */
public abstract class DbFieldManagerAppService : ApplicationService
{
    protected DbFieldManagerAppService()
    {
        LocalizationResource = typeof(DbFieldManagerResource);
    }
}
