using Sid.DbFieldManager.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Sid.DbFieldManager.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class DbFieldManagerController : AbpControllerBase
{
    protected DbFieldManagerController()
    {
        LocalizationResource = typeof(DbFieldManagerResource);
    }
}
