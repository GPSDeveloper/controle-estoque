using ControleEstoque.Models;
using ControleEstoque.Reports;
using ControleEstoque.UI;

namespace ControleEstoque.Forms;

public class RelatorioPreviewForm : Form
{
    private readonly RelatorioResultado _relatorio;
    private readonly bool _incluirLocalizacao;
    private readonly DataGridView _grid = new();

    public RelatorioPreviewForm(RelatorioResultado relatorio, bool incluirLocalizacao)
    {
        _relatorio = relatorio;
        _incluirLocalizacao = incluirLocalizacao;

        ThemeHelper.ConfigurarFormulario(this, "Visualizar Relatório", 900, 600);
        Controls.Add(ThemeHelper.CriarCabecalho());
        MontarInterface();
        CarregarDados();
    }

    private void MontarInterface()
    {
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 90, 20, 20) };

        var lblTitulo = new Label
        {
            Text = $"{_relatorio.Titulo} — {_relatorio.Subtitulo}",
            AutoSize = true,
            Font = ThemeHelper.FonteTitulo,
            Location = new Point(0, 0)
        };

        _grid.Location = new Point(0, 40);
        _grid.Size = new Size(840, 380);
        _grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;

        var lblTotal = new Label
        {
            AutoSize = true,
            Font = ThemeHelper.FonteBotao,
            Location = new Point(0, 430),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left
        };
        lblTotal.Text = $"Total Geral: {_relatorio.TotalGeral:C2}";

        var btnImprimir = ThemeHelper.CriarBotao("Imprimir", 120, 35);
        btnImprimir.Location = new Point(500, 430);
        btnImprimir.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnImprimir.Click += (_, _) => new PrintReportHelper(_relatorio, _incluirLocalizacao).Imprimir();

        var btnPdf = ThemeHelper.CriarBotao("Salvar PDF", 130, 35);
        btnPdf.Location = new Point(630, 430);
        btnPdf.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnPdf.Click += (_, _) => SalvarPdf();

        var btnFechar = ThemeHelper.CriarBotao("Fechar", 100, 35);
        btnFechar.Location = new Point(770, 430);
        btnFechar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnFechar.BackColor = Color.Gray;
        btnFechar.Click += (_, _) => Close();

        panel.Controls.AddRange(new Control[] { lblTitulo, _grid, lblTotal, btnImprimir, btnPdf, btnFechar });
        Controls.Add(panel);
    }

    private void CarregarDados()
    {
        if (_incluirLocalizacao)
        {
            _grid.DataSource = _relatorio.Itens.Select(i => new
            {
                i.MaterialNome,
                Quantidade = i.Quantidade.ToString("N2"),
                Localizacao = i.Localizacao ?? "-",
                PrecoUnitario = i.PrecoUnitario.ToString("C2"),
                PrecoTotal = i.PrecoTotal.ToString("C2")
            }).ToList();
        }
        else
        {
            _grid.DataSource = _relatorio.Itens.Select(i => new
            {
                i.MaterialNome,
                Quantidade = i.Quantidade.ToString("N2"),
                PrecoUnitario = i.PrecoUnitario.ToString("C2"),
                PrecoTotal = i.PrecoTotal.ToString("C2")
            }).ToList();
        }
    }

    private void SalvarPdf()
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "PDF (*.pdf)|*.pdf",
            FileName = $"relatorio_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
        };

        if (dialog.ShowDialog() != DialogResult.OK)
            return;

        try
        {
            PdfReportGenerator.Gerar(_relatorio, dialog.FileName, _incluirLocalizacao);
            MessageBox.Show($"Relatório salvo em:\n{dialog.FileName}", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao salvar PDF:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
