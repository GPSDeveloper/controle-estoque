using ControleEstoque.UI;

namespace ControleEstoque.Forms;

public class MainForm : Form
{
    public MainForm()
    {
        ThemeHelper.ConfigurarFormulario(this, "Controle de Estoque - SEMSRJ", 500, 580);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        var conteudo = ThemeHelper.CriarConteudoPrincipal(this, 420, new Padding(0));

        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };

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
            if (texto == "Sair")
                btn.BackColor = Color.DarkRed;
            btn.Click += (_, _) => acao();
            panel.Controls.Add(btn);
        }

        conteudo.Controls.Add(panel);
    }

    private static void AbrirForm<T>() where T : Form, new()
    {
        using var form = new T();
        form.ShowDialog();
    }
}
