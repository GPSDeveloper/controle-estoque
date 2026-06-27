using ControleEstoque.UI;

namespace ControleEstoque.Forms;

public class MainForm : Form
{
    public MainForm()
    {
        ThemeHelper.ConfigurarFormulario(this, "Controle de Estoque - SEMSRJ", 500, 580);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        var conteudo = ThemeHelper.CriarConteudoPrincipal(this, "Menu Principal", 420, new Padding(0));

        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1,
            Margin = new Padding(0),
            Padding = new Padding(0),
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        var botoes = new (string Texto, Action Acao)[]
        {
            ("Adicionar Setor", () => AbrirForm<SetorForm>()),
            ("Adicionar Material", () => AbrirForm<AdicionarMaterialForm>()),
            ("Saída de Material", () => AbrirForm<SaidaMaterialForm>()),
            ("Histórico de Saídas", () => AbrirForm<HistoricoSaidasForm>()),
            ("Estoque", () => AbrirForm<EstoqueForm>()),
            ("Gerar Relatórios", () => AbrirForm<RelatoriosForm>()),
            ("Sair", () => Close())
        };

        foreach (var (texto, acao) in botoes)
        {
            var btn = ThemeHelper.CriarBotao(texto, 400, 50);
            btn.Dock = DockStyle.Top;
            btn.Width = 0;
            if (texto == "Sair")
                btn.BackColor = Color.DarkRed;
            btn.Click += (_, _) => acao();
            var linha = panel.RowCount++;
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.Controls.Add(btn, 0, linha);
        }

        conteudo.Controls.Add(panel);
    }

    private static void AbrirForm<T>() where T : Form, new()
    {
        using var form = new T();
        form.ShowDialog();
    }
}
