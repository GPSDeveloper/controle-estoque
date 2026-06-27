using ControleEstoque.Data;

try
{
    using var context = DbContextFactory.Create();
    DatabaseInitializer.Initialize(context);
    Console.WriteLine("Migrations aplicadas com sucesso.");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Falha ao aplicar migrations: {ex.Message}");
    Environment.Exit(1);
}
