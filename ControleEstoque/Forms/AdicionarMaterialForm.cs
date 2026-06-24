using ControleEstoque.Services;
using ControleEstoque.UI;

namespace ControleEstoque.Forms;

public class AdicionarMaterialForm : Form
{
    private readonly MaterialService _service = new();
    private readonly TextBox _txtNome = new() { Width = 350 };
    private readonly NumericUpDown _numQuantidade = new() { Width = 150, DecimalPlaces = 2, Minimum = 0.01m, Maximum = 999999, Value = 1 };
    private readonly NumericUpDown _numPreco = new() { Width = 150, DecimalPlaces = 2, Minimum = 0, Maximum = 9999999, Value = 0 };
    private readonly TextBox _txtLocalizacao = new() { Width = 350 };
    private readonly CheckBox _chkValidade = new() { Text = "Possui data de validade", AutoSize = true };
    private readonly DateTimePicker _dtpValidade = new() { Width = 200, Enabled = false, Format = DateTimePickerFormat.Short };
    private Panel _conteudo = null!;

    public AdicionarMaterialForm()
    {
        ThemeHelper.ConfigurarFormulario(this, "Adicionar Material");
        _conteudo = ThemeHelper.CriarConteudoPrincipal(this, 760, new Padding(0));
        MontarInterface();
    }

    private void MontarInterface()
    {
        var formGrid = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 2,
            Margin = new Padding(0)
        };
        formGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));
        formGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        void AddField(string label, Control control)
        {
            var linha = formGrid.RowCount++;
            formGrid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            formGrid.Controls.Add(new Label { Text = label, AutoSize = true, Margin = new Padding(0, 8, 8, 0) }, 0, linha);
            control.Dock = DockStyle.Top;
            formGrid.Controls.Add(control, 1, linha);
        }

        AddField("Nome do material:", _txtNome);
        AddField("Quantidade:", _numQuantidade);
        AddField("Preço unitário (R$):", _numPreco);
        AddField("Localização:", _txtLocalizacao);

        _chkValidade.CheckedChanged += (_, _) => _dtpValidade.Enabled = _chkValidade.Checked;
        var linhaCheck = formGrid.RowCount++;
        formGrid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        formGrid.Controls.Add(_chkValidade, 1, linhaCheck);

        AddField("Data de validade:", _dtpValidade);

        var btnAdicionar = ThemeHelper.CriarBotao("Adicionar", 150, 40);
        btnAdicionar.Click += (_, _) => Adicionar();

        var btnFechar = ThemeHelper.CriarBotao("Fechar", 120, 40);
        btnFechar.BackColor = Color.Gray;
        btnFechar.Click += (_, _) => Close();

        var barraAcoes = ThemeHelper.CriarBarraAcoesInferior();
        barraAcoes.Controls.Add(btnFechar);
        barraAcoes.Controls.Add(btnAdicionar);

        _conteudo.Controls.Add(barraAcoes);
        _conteudo.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 12 });
        _conteudo.Controls.Add(formGrid);
    }

    private void Adicionar()
    {
        OperationFeedbackHelper.ExecutarComRetryConcorrencia(
            () => _service.Adicionar(
                _txtNome.Text,
                _numQuantidade.Value,
                _numPreco.Value,
                _txtLocalizacao.Text,
                _chkValidade.Checked,
                _chkValidade.Checked ? _dtpValidade.Value : null),
            onSuccess: () =>
            {
                _txtNome.Clear();
                _numQuantidade.Value = 1;
                _numPreco.Value = 0;
                _txtLocalizacao.Clear();
                _chkValidade.Checked = false;
            });
    }
}
