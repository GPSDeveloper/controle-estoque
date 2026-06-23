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

    public AdicionarMaterialForm()
    {
        ThemeHelper.ConfigurarFormulario(this, "Adicionar Material");
        Controls.Add(ThemeHelper.CriarCabecalho());
        MontarInterface();
    }

    private void MontarInterface()
    {
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(30, 90, 30, 20) };
        int y = 0;

        void AddField(string label, Control control)
        {
            panel.Controls.Add(new Label { Text = label, AutoSize = true, Location = new Point(0, y) });
            control.Location = new Point(0, y + 22);
            panel.Controls.Add(control);
            y += 55;
        }

        AddField("Nome do material:", _txtNome);
        AddField("Quantidade:", _numQuantidade);
        AddField("Preço unitário (R$):", _numPreco);
        AddField("Localização:", _txtLocalizacao);

        _chkValidade.Location = new Point(0, y);
        _chkValidade.CheckedChanged += (_, _) => _dtpValidade.Enabled = _chkValidade.Checked;
        panel.Controls.Add(_chkValidade);
        y += 30;

        panel.Controls.Add(new Label { Text = "Data de validade:", AutoSize = true, Location = new Point(0, y) });
        _dtpValidade.Location = new Point(0, y + 22);
        panel.Controls.Add(_dtpValidade);
        y += 60;

        var btnAdicionar = ThemeHelper.CriarBotao("Adicionar", 150, 40);
        btnAdicionar.Location = new Point(0, y);
        btnAdicionar.Click += (_, _) => Adicionar();

        var btnFechar = ThemeHelper.CriarBotao("Fechar", 120, 40);
        btnFechar.Location = new Point(160, y);
        btnFechar.BackColor = Color.Gray;
        btnFechar.Click += (_, _) => Close();

        panel.Controls.AddRange(new Control[] { btnAdicionar, btnFechar });
        Controls.Add(panel);
    }

    private void Adicionar()
    {
        var (sucesso, mensagem) = _service.Adicionar(
            _txtNome.Text,
            _numQuantidade.Value,
            _numPreco.Value,
            _txtLocalizacao.Text,
            _chkValidade.Checked,
            _chkValidade.Checked ? _dtpValidade.Value : null);

        MessageBox.Show(mensagem, sucesso ? "Sucesso" : "Atenção", MessageBoxButtons.OK,
            sucesso ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

        if (sucesso)
        {
            _txtNome.Clear();
            _numQuantidade.Value = 1;
            _numPreco.Value = 0;
            _txtLocalizacao.Clear();
            _chkValidade.Checked = false;
        }
    }
}
