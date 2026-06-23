using Microsoft.EntityFrameworkCore;

namespace ControleEstoque.Data;

public static class DbContextFactory
{
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(AppConfiguration.ConnectionString)
            .Options;

        return new AppDbContext(options);
    }
}
