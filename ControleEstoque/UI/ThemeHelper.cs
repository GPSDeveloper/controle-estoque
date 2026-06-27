namespace ControleEstoque.UI;

public static class ThemeHelper
{
    public const string Instituicao = "Ministério da Saúde / SEMSRJ / Almoxarifado";
    public const int Espacamento = 8;
    public static readonly Color CorPrimaria = Color.FromArgb(0, 51, 102);
    public static readonly Color CorSecundaria = Color.FromArgb(240, 244, 248);
    public static readonly Font FonteTitulo = new("Segoe UI", 14F, FontStyle.Bold);
    public static readonly Font FonteSubtitulo = new("Segoe UI", 10F, FontStyle.Regular);
    public static readonly Font FonteBotao = new("Segoe UI", 10F, FontStyle.Bold);

    public static Panel CriarCabecalho(string tituloFluxo)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 70,
            BackColor = CorPrimaria
        };

        var lbl = new Label
        {
            Text = tituloFluxo,
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
        form.MinimumSize = new Size(Math.Min(largura, 700), Math.Min(altura, 500));
    }

    public static Panel CriarConteudoPrincipal(
        Form form,
        string tituloFluxo,
        int larguraMaxima = 1000,
        Padding? paddingConteudo = null)
    {
        var areaRolagem = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(24)
        };
        areaRolagem.Padding = paddingConteudo ?? new Padding(24);

        form.Controls.Add(areaRolagem);
        form.Controls.Add(CriarCabecalho(tituloFluxo));

        return areaRolagem;
    }

    public static FlowLayoutPanel CriarBarraAcoesInferior()
    {
        return new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 52,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Margin = new Padding(0),
            Padding = new Padding(0, Espacamento, 0, 0)
        };
    }
}
