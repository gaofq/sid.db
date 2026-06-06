using Microsoft.EntityFrameworkCore;
using Sid.DbFieldManager.DbFields;
using Sid.DbFieldManager.DbTables;
using Sid.DbFieldManager.SqlExecutionLogs;
using Sid.DbFieldManager.TargetDatabases;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Sid.DbFieldManager.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class DbFieldManagerDbContext :
    AbpDbContext<DbFieldManagerDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    public DbSet<TargetDatabase> TargetDatabases { get; set; }
    public DbSet<DbTable> DbTables { get; set; }
    public DbSet<DbField> DbFields { get; set; }
    public DbSet<SqlExecutionLog> SqlExecutionLogs { get; set; }

    #region Entities from the modules

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }
    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public DbFieldManagerDbContext(DbContextOptions<DbFieldManagerDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        builder.Entity<TargetDatabase>(b =>
        {
            b.ToTable("AppTargetDatabases");
            b.Property(x => x.Name).IsRequired().HasMaxLength(128);
            b.Property(x => x.ConnectionString).IsRequired().HasMaxLength(500);
            b.Property(x => x.Description).HasMaxLength(256);
        });

        builder.Entity<DbTable>(b =>
        {
            b.ToTable("AppDbTables");
            b.HasIndex(x => x.Name);
            b.Property(x => x.Name).IsRequired().HasMaxLength(128);
            b.Property(x => x.DisplayName).HasMaxLength(256);
            b.Property(x => x.Schema).HasMaxLength(128).HasDefaultValue("dbo");
            b.HasOne(x => x.TargetDatabase)
             .WithMany(x => x.Tables)
             .HasForeignKey(x => x.TargetDatabaseId)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.SetNull);
            b.HasMany(x => x.Fields)
             .WithOne(x => x.DbTable)
             .HasForeignKey(x => x.DbTableId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<DbField>(b =>
        {
            b.ToTable("AppDbFields");
            b.HasIndex(x => new { x.DbTableId, x.Name }).IsUnique();
            b.Property(x => x.Name).IsRequired().HasMaxLength(128);
            b.Property(x => x.SqlType).IsRequired().HasMaxLength(50);
            b.Property(x => x.DefaultValue).HasMaxLength(200);
            b.Property(x => x.Description).HasMaxLength(500);
            b.Property(x => x.ExecutionStatus).HasDefaultValue(ExecutionStatus.Pending);
        });

        builder.Entity<SqlExecutionLog>(b =>
        {
            b.ToTable("AppSqlExecutionLogs");
            b.Property(x => x.SqlScript).IsRequired();
            b.Property(x => x.ErrorMessage).HasMaxLength(4000);
            b.HasIndex(x => x.TargetDatabaseId);
            b.HasIndex(x => x.CreationTime);
        });
    }
}
