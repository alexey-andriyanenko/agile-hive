using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace OrganizationService.Infrastructure.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../OrganizationService.API"))
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = configuration.GetConnectionString("OrganizationDb");

        optionsBuilder.UseNpgsql(connectionString);
        
        var tenantContext = new TenantContext
        {
            TenantId = Guid.Empty,
            ServiceName = "OrganizationService",
            DbConnectionString = connectionString!,
        };

        return new ApplicationDbContext(optionsBuilder.Options, tenantContext);
    }
}