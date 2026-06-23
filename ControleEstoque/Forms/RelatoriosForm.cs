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

    public RelatoriosForm()
    {
        ThemeHelper.ConfigurarFormulario(this, "Gerar Relatórios", 550, 450);
        Controls.Add(ThemeHelper.CriarCabecalho());
        MontarInterface();
        CarregarSetores();
    }

    private void MontarInterface()
    {
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 90, 20, 20) };

        _tabs.TabPages.Add(CriarAbaMensal());
        _tabs.TabPages.Add(CriarAbaAnual());
        _tabs.TabPages.Add(CriarAbaInventario());

        var btnFechar = ThemeHelper.CriarBotao("Fechar", 120, 35);
        btnFechar.Dock = DockStyle.Bottom;
        btnFechar.BackColor = Color.Gray;
        btnFechar.Click += (_, _) => Close();

        panel.Controls.Add(_tabs);
        Controls.Add(panel);
        Controls.Add(btnFechar);
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
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

        _cmbSetorMensal = new ComboBox { Width = 300, DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(0, 30) };
        _cmbMes = new ComboBox { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(0, 90) };
        _numAnoMensal = new NumericUpDown { Width = 100, Minimum = 2026, Maximum = 2100, Value = DateTime.Now.Year, Location = new Point(220, 90) };

        var cultura = CultureInfo.GetCultureInfo("pt-BR");
        for (int i = 1; i <= 12; i++)
            _cmbMes.Items.Add(cultura.DateTimeFormat.GetMonthName(i));
        _cmbMes.SelectedIndex = DateTime.Now.Month - 1;

        panel.Controls.Add(new Label { Text = "Setor:", AutoSize = true, Location = new Point(0, 10) });
        panel.Controls.Add(_cmbSetorMensal);
        panel.Controls.Add(new Label { Text = "Mês / Ano:", AutoSize = true, Location = new Point(0, 70) });
        panel.Controls.Add(_cmbMes);
        panel.Controls.Add(_numAnoMensal);

        var btnGerar = ThemeHelper.CriarBotao("Gerar Relatório Mensal", 220, 40);
        btnGerar.Location = new Point(0, 140);
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
        panel.Controls.Add(btnGerar);
        tab.Controls.Add(panel);
        return tab;
    }

    private TabPage CriarAbaAnual()
    {
        var tab = new TabPage("Anual");
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

        _cmbSetorAnual = new ComboBox { Width = 300, DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(0, 30) };
        _numAnoAnual = new NumericUpDown { Width = 100, Minimum = 2026, Maximum = 2100, Value = DateTime.Now.Year, Location = new Point(0, 90) };

        panel.Controls.Add(new Label { Text = "Setor:", AutoSize = true, Location = new Point(0, 10) });
        panel.Controls.Add(_cmbSetorAnual);
        panel.Controls.Add(new Label { Text = "Ano:", AutoSize = true, Location = new Point(0, 70) });
        panel.Controls.Add(_numAnoAnual);

        var btnGerar = ThemeHelper.CriarBotao("Gerar Relatório Anual", 220, 40);
        btnGerar.Location = new Point(0, 140);
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
        panel.Controls.Add(btnGerar);
        tab.Controls.Add(panel);
        return tab;
    }

    private TabPage CriarAbaInventario()
    {
        var tab = new TabPage("Inventário");
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

        var lbl = new Label
        {
            Text = "Gera relatório completo do estoque atual com localização e valores.",
            AutoSize = true,
            Location = new Point(0, 20)
        };

        var btnGerar = ThemeHelper.CriarBotao("Gerar Relatório de Inventário", 250, 40);
        btnGerar.Location = new Point(0, 60);
        btnGerar.Click += (_, _) =>
        {
            var relatorio = _service.GerarInventario();
            AbrirPreview(relatorio, true);
        };

        panel.Controls.AddRange(new Control[] { lbl, btnGerar });
        tab.Controls.Add(panel);
        return tab;
    }

    private static void AbrirPreview(RelatorioResultado relatorio, bool incluirLocalizacao)
    {
        using var preview = new RelatorioPreviewForm(relatorio, incluirLocalizacao);
        preview.ShowDialog();
    }
}
