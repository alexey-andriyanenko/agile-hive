using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BoardService.Infrastructure.Data;

public class DbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../BoardService.API"))
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = configuration.GetConnectionString("BoardDb");
        
        optionsBuilder.UseNpgsql(connectionString);
        
        var tenantContext = new TenantContext
        {
            TenantId = Guid.Empty,
            ServiceName = "BoardService",
            DbConnectionString = connectionString!,
        };
        
        return new ApplicationDbContext(optionsBuilder.Options, tenantContext);
    }
}