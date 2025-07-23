using IdentityService.Contracts.Protos;
using Web.API.DI;

namespace Web.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddGrpcClient<Auth.AuthClient>(options =>
        {
            options.Address = new Uri("http://localhost:5243");
        });
        builder.Services.AddServices(builder.Configuration);
        builder.Services.AddControllers();
        
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}