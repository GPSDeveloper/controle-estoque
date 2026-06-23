using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ControleEstoque.Data;

public class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var projectDirectory = Path.Combine(currentDirectory, "ControleEstoque");
        var basePath = File.Exists(Path.Combine(projectDirectory, "appsettings.json"))
            ? projectDirectory
            : currentDirectory;

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .Build();

        var connectionString =
            Environment.GetEnvironmentVariable("CONNECTION_STRING")
            ?? configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string não configurada para design-time.");

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}
