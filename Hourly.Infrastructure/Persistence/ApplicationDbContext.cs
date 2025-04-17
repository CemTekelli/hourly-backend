// ApplicationDbContext.cs
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Organization> Organizations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuration des clés composites
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        // Relations
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Organization)
            .WithMany(o => o.Users)
            .HasForeignKey(u => u.OrganizationId);

        // Seed des données initiales (rôles et permissions)
        SeedInitialData(modelBuilder);
    }

    private void SeedInitialData(ModelBuilder modelBuilder)
    {
        // Rôles de base
        var roles = new[]
        {
            new Role { Id = Guid.NewGuid(), Name = "SuperAdmin", Description = "Super administrateur système" },
            new Role { Id = Guid.NewGuid(), Name = "OrganizationAdmin", Description = "Administrateur d'entreprise" },
            new Role { Id = Guid.NewGuid(), Name = "ResourceManager", Description = "Gestionnaire des ressources" },
            new Role { Id = Guid.NewGuid(), Name = "SalesManager", Description = "Gestionnaire commercial" },
            new Role { Id = Guid.NewGuid(), Name = "FinanceManager", Description = "Gestionnaire financier" },
            new Role { Id = Guid.NewGuid(), Name = "Consultant", Description = "Consultant" },
            new Role { Id = Guid.NewGuid(), Name = "ExternalClient", Description = "Client externe" }
        };
        
        modelBuilder.Entity<Role>().HasData(roles);

        // Permission modules
        var modules = new[] { "Users", "Organizations", "Consultants", "Projects", "Timesheets", "Invoices", "Reports" };
        
        // Permissions par module
        var permissions = new List<Permission>();
        var actionTypes = new[] { "View", "Create", "Edit", "Delete" };
        
        var permissionId = 0;
        foreach (var module in modules)
        {
            foreach (var action in actionTypes)
            {
                permissions.Add(new Permission { 
                    Id = Guid.NewGuid(), 
                    Name = $"{action}{module}", 
                    Description = $"{action} {module}", 
                    Module = module 
                });
            }
        }
        
        modelBuilder.Entity<Permission>().HasData(permissions);
    }
}