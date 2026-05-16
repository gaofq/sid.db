using Microsoft.Extensions.Localization;
using Sid.DbFieldManager.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Sid.DbFieldManager;

[Dependency(ReplaceServices = true)]
public class DbFieldManagerBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<DbFieldManagerResource> _localizer;

    public DbFieldManagerBrandingProvider(IStringLocalizer<DbFieldManagerResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
