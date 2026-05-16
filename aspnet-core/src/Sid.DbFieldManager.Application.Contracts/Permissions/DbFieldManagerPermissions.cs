namespace Sid.DbFieldManager.Permissions;

public static class DbFieldManagerPermissions
{
    public const string GroupName = "DbFieldManager";

    public static class TargetDatabases
    {
        public const string Default = GroupName + ".TargetDatabases";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class DbTables
    {
        public const string Default = GroupName + ".DbTables";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class DbFields
    {
        public const string Default = GroupName + ".DbFields";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class SqlGeneration
    {
        public const string Default = GroupName + ".SqlGeneration";
    }

    public static class SqlExecution
    {
        public const string Default = GroupName + ".SqlExecution";
    }
}
