namespace ControleEstoque.UI;

public static class ThemeHelper
{
    public const string Instituicao = "Ministério da Saúde / SEMSRJ / Almoxarifado";
    public static readonly Color CorPrimaria = Color.FromArgb(0, 51, 102);
    public static readonly Color CorSecundaria = Color.FromArgb(240, 244, 248);
    public static readonly Font FonteTitulo = new("Segoe UI", 14F, FontStyle.Bold);
    public static readonly Font FonteSubtitulo = new("Segoe UI", 10F, FontStyle.Regular);
    public static readonly Font FonteBotao = new("Segoe UI", 10F, FontStyle.Bold);

    public static Panel CriarCabecalho()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 70,
            BackColor = CorPrimaria
        };

        var lbl = new Label
        {
            Text = Instituicao,
            ForeColor = Color.White,
            Font = FonteTitulo,
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        };

        panel.Controls.Add(lbl);
        return panel;
    }

    public static Button CriarBotao(string texto, int largura = 220, int altura = 45)
    {
        return new Button
        {
            Text = texto,
            Width = largura,
            Height = altura,
            FlatStyle = FlatStyle.Flat,
            BackColor = CorPrimaria,
            ForeColor = Color.White,
            Font = FonteBotao,
            Cursor = Cursors.Hand,
            Margin = new Padding(8)
        };
    }

    public static void ConfigurarFormulario(Form form, string titulo, int largura = 900, int altura = 600)
    {
        form.Text = titulo;
        form.Size = new Size(largura, altura);
        form.StartPosition = FormStartPosition.CenterScreen;
        form.BackColor = CorSecundaria;
        form.Font = FonteSubtitulo;
        form.MinimumSize = new Size(700, 500);
    }
}
