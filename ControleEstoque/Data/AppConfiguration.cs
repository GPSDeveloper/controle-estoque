using Microsoft.Extensions.Configuration;

namespace ControleEstoque.Data;

public static class AppConfiguration
{
  private static IConfiguration? _configuration;

  public static IConfiguration Configuration
  {
    get
    {
      if (_configuration == null)
      {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
      }

      return _configuration;
    }
  }

  public static string ConnectionString =>
      Environment.GetEnvironmentVariable("CONNECTION_STRING")
      ?? Configuration.GetConnectionString("Default")
      ?? throw new InvalidOperationException("Connection string 'Default' não configurada.");
}
