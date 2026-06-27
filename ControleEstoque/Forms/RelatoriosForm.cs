using ControleEstoque.Models;
using ControleEstoque.Services;
using ControleEstoque.UI;
using System.Globalization;

namespace ControleEstoque.Forms;

public class RelatoriosForm : Form
{
    private readonly RelatorioService _service = new();
    private readonly SetorService _setorService = new();
    private readonly TabControl _tabs = new() { Dock = DockStyle.Fill };

    private ComboBox _cmbSetorMensal = new();
    private ComboBox _cmbMes = new();
    private NumericUpDown _numAnoMensal = new();
    private ComboBox _cmbSetorAnual = new();
    private NumericUpDown _numAnoAnual = new();
    private Panel _conteudo = null!;

    public RelatoriosForm()
    {
        ThemeHelper.ConfigurarFormulario(this, "Gerar Relatórios", 550, 450);
        _conteudo = ThemeHelper.CriarConteudoPrincipal(this, 760, new Padding(0));
        MontarInterface();
        CarregarSetores();
    }

    private void MontarInterface()
    {
        _tabs.TabPages.Add(CriarAbaMensal());
        _tabs.TabPages.Add(CriarAbaAnual());
        _tabs.TabPages.Add(CriarAbaInventario());
        _tabs.Dock = DockStyle.Top;
        _tabs.Height = 360;

        var btnFechar = ThemeHelper.CriarBotao("Fechar", 120, 35);
        btnFechar.BackColor = Color.Gray;
        btnFechar.Click += (_, _) => Close();

        var barraAcoes = ThemeHelper.CriarBarraAcoesInferior();
        barraAcoes.Controls.Add(btnFechar);

        _conteudo.Controls.Add(barraAcoes);
        _conteudo.Controls.Add(_tabs);
    }

    private void CarregarSetores()
    {
        var setores = _setorService.ListarTodos();
        _cmbSetorMensal.DataSource = new List<Setor>(setores);
        _cmbSetorMensal.DisplayMember = "Nome";
        _cmbSetorMensal.ValueMember = "Id";

        _cmbSetorAnual.DataSource = new List<Setor>(setores);
        _cmbSetorAnual.DisplayMember = "Nome";
        _cmbSetorAnual.ValueMember = "Id";
    }

    private TabPage CriarAbaMensal()
    {
        var tab = new TabPage("Mensal");
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20),
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 2
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        _cmbSetorMensal = new ComboBox { Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbMes = new ComboBox { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
        _numAnoMensal = new NumericUpDown { Width = 100, Minimum = 2026, Maximum = 2100, Value = DateTime.Now.Year };

        var cultura = CultureInfo.GetCultureInfo("pt-BR");
        for (int i = 1; i <= 12; i++)
            _cmbMes.Items.Add(cultura.DateTimeFormat.GetMonthName(i));
        _cmbMes.SelectedIndex = DateTime.Now.Month - 1;

        var linha0 = panel.RowCount++;
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.Controls.Add(new Label { Text = "Setor:", AutoSize = true, Margin = new Padding(0, 8, 8, 0) }, 0, linha0);
        panel.Controls.Add(_cmbSetorMensal, 1, linha0);

        var linha1 = panel.RowCount++;
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.Controls.Add(new Label { Text = "Mês / Ano:", AutoSize = true, Margin = new Padding(0, 8, 8, 0) }, 0, linha1);
        var mesAnoPanel = new FlowLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Dock = DockStyle.Top,
            WrapContents = false
        };
        mesAnoPanel.Controls.Add(_cmbMes);
        mesAnoPanel.Controls.Add(_numAnoMensal);
        panel.Controls.Add(mesAnoPanel, 1, linha1);

        var btnGerar = ThemeHelper.CriarBotao("Gerar Relatório Mensal", 220, 40);
        btnGerar.Click += (_, _) =>
        {
            if (_cmbSetorMensal.SelectedValue is not int setorId)
            {
                MessageBox.Show("Selecione um setor.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var relatorio = _service.GerarMensal(setorId, _cmbMes.SelectedIndex + 1, (int)_numAnoMensal.Value);
            AbrirPreview(relatorio, false);
        };
        var linha2 = panel.RowCount++;
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.Controls.Add(new Label(), 0, linha2);
        panel.Controls.Add(btnGerar, 1, linha2);
        tab.Controls.Add(panel);
        return tab;
    }

    private TabPage CriarAbaAnual()
    {
        var tab = new TabPage("Anual");
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20),
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 2
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        _cmbSetorAnual = new ComboBox { Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };
        _numAnoAnual = new NumericUpDown { Width = 100, Minimum = 2026, Maximum = 2100, Value = DateTime.Now.Year };

        var linha0 = panel.RowCount++;
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.Controls.Add(new Label { Text = "Setor:", AutoSize = true, Margin = new Padding(0, 8, 8, 0) }, 0, linha0);
        panel.Controls.Add(_cmbSetorAnual, 1, linha0);

        var linha1 = panel.RowCount++;
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.Controls.Add(new Label { Text = "Ano:", AutoSize = true, Margin = new Padding(0, 8, 8, 0) }, 0, linha1);
        panel.Controls.Add(_numAnoAnual, 1, linha1);

        var btnGerar = ThemeHelper.CriarBotao("Gerar Relatório Anual", 220, 40);
        btnGerar.Click += (_, _) =>
        {
            if (_cmbSetorAnual.SelectedValue is not int setorId)
            {
                MessageBox.Show("Selecione um setor.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var relatorio = _service.GerarAnual(setorId, (int)_numAnoAnual.Value);
            AbrirPreview(relatorio, false);
        };
        var linha2 = panel.RowCount++;
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.Controls.Add(new Label(), 0, linha2);
        panel.Controls.Add(btnGerar, 1, linha2);
        tab.Controls.Add(panel);
        return tab;
    }

    private TabPage CriarAbaInventario()
    {
        var tab = new TabPage("Inventário");
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20),
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1
        };

        var lbl = new Label
        {
            Text = "Gera relatório completo do estoque atual com localização e valores.",
            AutoSize = true,
            Margin = new Padding(0, 8, 0, 8)
        };

        var btnGerar = ThemeHelper.CriarBotao("Gerar Relatório de Inventário", 250, 40);
        btnGerar.Margin = new Padding(0, 4, 0, 0);
        btnGerar.Click += (_, _) =>
        {
            var relatorio = _service.GerarInventario();
            AbrirPreview(relatorio, true);
        };

        panel.Controls.Add(lbl);
        panel.Controls.Add(btnGerar);
        tab.Controls.Add(panel);
        return tab;
    }

    private static void AbrirPreview(RelatorioResultado relatorio, bool incluirLocalizacao)
    {
        using var preview = new RelatorioPreviewForm(relatorio, incluirLocalizacao);
        preview.ShowDialog();
    }
}
