using ControleEstoque.Models;
using ControleEstoque.Services;
using ControleEstoque.UI;

namespace ControleEstoque.Forms;

public class EditarSaidaForm : Form
{
    private readonly SaidaHistoricoItem _item;
    private readonly MaterialService _materialService = new();
    private readonly SetorService _setorService = new();

    private readonly ComboBox _cmbMaterial = new() { Width = 350, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly NumericUpDown _numQuantidade = new() { Width = 150, DecimalPlaces = 2, Minimum = 0.01m, Maximum = 999999 };
    private readonly TextBox _txtNome = new() { Width = 300 };
    private readonly RadioButton _rbCpf = new() { Text = "CPF", AutoSize = true };
    private readonly RadioButton _rbMatricula = new() { Text = "Matrícula", AutoSize = true };
    private readonly TextBox _txtIdentificacao = new() { Width = 250 };
    private readonly ComboBox _cmbSetor = new() { Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };

    public EditarSaidaForm(SaidaHistoricoItem item)
    {
        _item = item;
        Text = "Editar Saída";
        Size = new Size(450, 420);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        int y = 15;
        void Add(string label, Control c)
        {
            Controls.Add(new Label { Text = label, AutoSize = true, Location = new Point(20, y) });
            c.Location = new Point(20, y + 20);
            Controls.Add(c);
            y += 50;
        }

        var materiais = _materialService.ListarTodos();
        _cmbMaterial.DataSource = materiais;
        _cmbMaterial.DisplayMember = "Nome";
        _cmbMaterial.ValueMember = "Id";
        _cmbMaterial.SelectedValue = item.MaterialId;

        _numQuantidade.Value = item.Quantidade;
        _txtNome.Text = item.NomeRetirante;
        _txtIdentificacao.Text = item.Identificacao;
        _rbCpf.Checked = item.TipoIdentificacao == "CPF";
        _rbMatricula.Checked = ! _rbCpf.Checked;

        var setores = _setorService.ListarTodos();
        _cmbSetor.DataSource = setores;
        _cmbSetor.DisplayMember = "Nome";
        _cmbSetor.ValueMember = "Id";
        _cmbSetor.SelectedValue = item.SetorId;

        Add("Material:", _cmbMaterial);
        Add("Quantidade:", _numQuantidade);
        Add("Nome:", _txtNome);

        Controls.Add(new Label { Text = "Identificação:", AutoSize = true, Location = new Point(20, y) });
        _rbCpf.Location = new Point(20, y + 20);
        _rbMatricula.Location = new Point(100, y + 20);
        Controls.Add(_rbCpf);
        Controls.Add(_rbMatricula);
        y += 45;
        Add("CPF/Matrícula:", _txtIdentificacao);
        Add("Setor:", _cmbSetor);

        var btnSalvar = new Button { Text = "Salvar", DialogResult = DialogResult.OK, Location = new Point(200, y), Width = 80 };
        var btnCancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Location = new Point(290, y), Width = 80 };
        Controls.AddRange(new Control[] { btnSalvar, btnCancelar });
        AcceptButton = btnSalvar;
        CancelButton = btnCancelar;
    }

    public (int MaterialId, decimal Quantidade, string Nome, TipoIdentificacao Tipo, string Identificacao, int SetorId) ObterDados()
    {
        var tipo = _rbCpf.Checked ? TipoIdentificacao.CPF : TipoIdentificacao.Matricula;
        return (
            (int)(_cmbMaterial.SelectedValue ?? _item.MaterialId),
            _numQuantidade.Value,
            _txtNome.Text,
            tipo,
            _txtIdentificacao.Text,
            (int)(_cmbSetor.SelectedValue ?? _item.SetorId)
        );
    }
}
