using ControleEstoque.UI;

namespace ControleEstoque.Forms;

public class MainForm : Form
{
    public MainForm()
    {
        ThemeHelper.ConfigurarFormulario(this, "Controle de Estoque - SEMSRJ", 500, 580);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        Controls.Add(ThemeHelper.CriarCabecalho());

        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            Padding = new Padding(30, 90, 30, 20)
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
            var btn = ThemeHelper.CriarBotao(texto, 380, 50);
            if (texto == "Sair")
                btn.BackColor = Color.DarkRed;
            btn.Click += (_, _) => acao();
            panel.Controls.Add(btn);
        }

        Controls.Add(panel);
    }

    private static void AbrirForm<T>() where T : Form, new()
    {
        using var form = new T();
        form.ShowDialog();
    }
}
