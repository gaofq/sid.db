using Sid.DbFieldManager.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Sid.DbFieldManager.Permissions;

public class DbFieldManagerPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(DbFieldManagerPermissions.GroupName, L("DbFieldManager"));

        var targetDatabases = myGroup.AddPermission(DbFieldManagerPermissions.TargetDatabases.Default, L("TargetDatabases"));
        targetDatabases.AddChild(DbFieldManagerPermissions.TargetDatabases.Create, L("Create"));
        targetDatabases.AddChild(DbFieldManagerPermissions.TargetDatabases.Update, L("Update"));
        targetDatabases.AddChild(DbFieldManagerPermissions.TargetDatabases.Delete, L("Delete"));

        var dbTables = myGroup.AddPermission(DbFieldManagerPermissions.DbTables.Default, L("DbTables"));
        dbTables.AddChild(DbFieldManagerPermissions.DbTables.Create, L("Create"));
        dbTables.AddChild(DbFieldManagerPermissions.DbTables.Update, L("Update"));
        dbTables.AddChild(DbFieldManagerPermissions.DbTables.Delete, L("Delete"));

        var dbFields = myGroup.AddPermission(DbFieldManagerPermissions.DbFields.Default, L("DbFields"));
        dbFields.AddChild(DbFieldManagerPermissions.DbFields.Create, L("Create"));
        dbFields.AddChild(DbFieldManagerPermissions.DbFields.Update, L("Update"));
        dbFields.AddChild(DbFieldManagerPermissions.DbFields.Delete, L("Delete"));

        myGroup.AddPermission(DbFieldManagerPermissions.SqlGeneration.Default, L("SqlGeneration"));
        myGroup.AddPermission(DbFieldManagerPermissions.SqlExecution.Default, L("SqlExecution"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<DbFieldManagerResource>(name);
    }
}
