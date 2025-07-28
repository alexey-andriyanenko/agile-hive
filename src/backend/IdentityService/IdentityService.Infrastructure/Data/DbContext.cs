using IdentityService.Domain.Constants;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options)
    { }
    
    public DbSet<Tenant>  Tenants { get; set; }
    
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new UserConfiguration());
        builder.ApplyConfiguration(new RoleConfiguration());
        builder.ApplyConfiguration(new TenantConfiguration());
        
        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSeeding((context, _) => { });
        
        optionsBuilder.UseAsyncSeeding(async (context, _, cancellationToken) =>
        {
            var rolesContext = context.Set<Role>();
            var existingRoles = await rolesContext.ToListAsync(cancellationToken);

            if (existingRoles.Count > 0)
            {
                return;
            }

            List<Role> roles = [AppRoles.Admin, AppRoles.Manager, AppRoles.Employee];

            await rolesContext.AddRangeAsync(roles, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        });
    }
}
