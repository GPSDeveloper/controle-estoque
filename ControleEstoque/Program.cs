using ControleEstoque.Data;
using ControleEstoque.Forms;
using QuestPDF.Infrastructure;

namespace ControleEstoque;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        QuestPDF.Settings.License = LicenseType.Community;

        try
        {
            using var context = DbContextFactory.Create();
            DatabaseInitializer.Initialize(context);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Erro ao conectar ao banco de dados:\n{ex.Message}\n\nVerifique o PostgreSQL e o arquivo appsettings.json.",
                "Erro de Conexão",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        Application.Run(new LoginForm());
    }
}
