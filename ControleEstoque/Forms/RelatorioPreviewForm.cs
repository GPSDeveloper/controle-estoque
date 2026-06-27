using ControleEstoque.Models;
using ControleEstoque.Reports;
using ControleEstoque.UI;

namespace ControleEstoque.Forms;

public class RelatorioPreviewForm : Form
{
    private readonly RelatorioResultado _relatorio;
    private readonly bool _incluirLocalizacao;
    private readonly DataGridView _grid = new();
    private Panel _conteudo = null!;

    public RelatorioPreviewForm(RelatorioResultado relatorio, bool incluirLocalizacao)
    {
        _relatorio = relatorio;
        _incluirLocalizacao = incluirLocalizacao;

        ThemeHelper.ConfigurarFormulario(this, "Visualizar Relatório", 900, 600);
        _conteudo = ThemeHelper.CriarConteudoPrincipal(this, "Gerar Relatórios", 1200, new Padding(0));
        MontarInterface();
        CarregarDados();
    }

    private void MontarInterface()
    {
        var lblTitulo = new Label
        {
            Text = $"{_relatorio.Titulo} — {_relatorio.Subtitulo}",
            AutoSize = true,
            Font = ThemeHelper.FonteTitulo,
            Dock = DockStyle.Top,
            Margin = new Padding(0, 0, 0, 8)
        };

        _grid.Dock = DockStyle.Fill;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;

        var lblTotal = new Label
        {
            AutoSize = true,
            Font = ThemeHelper.FonteBotao,
            Margin = new Padding(0, 8, 0, 0)
        };
        lblTotal.Text = $"Total Geral: {_relatorio.TotalGeral:C2}";

        var btnImprimir = ThemeHelper.CriarBotao("Imprimir", 120, 35);
        btnImprimir.Click += (_, _) => new PrintReportHelper(_relatorio, _incluirLocalizacao).Imprimir();

        var btnPdf = ThemeHelper.CriarBotao("Salvar PDF", 130, 35);
        btnPdf.Click += (_, _) => SalvarPdf();

        var btnFechar = ThemeHelper.CriarBotao("Fechar", 100, 35);
        btnFechar.BackColor = Color.Gray;
        btnFechar.Click += (_, _) => Close();

        var barraAcoes = ThemeHelper.CriarBarraAcoesInferior();
        barraAcoes.Controls.Add(btnFechar);
        barraAcoes.Controls.Add(btnPdf);
        barraAcoes.Controls.Add(btnImprimir);

        var areaPrincipal = new Panel { Dock = DockStyle.Fill };
        areaPrincipal.Controls.Add(_grid);
        areaPrincipal.Controls.Add(lblTitulo);

        _conteudo.Controls.Add(barraAcoes);
        _conteudo.Controls.Add(lblTotal);
        _conteudo.Controls.Add(areaPrincipal);
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
