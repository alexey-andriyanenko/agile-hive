using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TagService.Infrastructure.Data;

public class DbContextDesignFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../TagService.API"))
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = configuration.GetConnectionString("TagDb");
        
        optionsBuilder.UseNpgsql(connectionString);

        var tenantContext = new TenantContext
        {
            TenantId = Guid.Empty,
            ServiceName = "TagDb",
            DbConnectionString = connectionString!,
        };
        
        return new ApplicationDbContext(optionsBuilder.Options, tenantContext);
    }
}
