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
        Size = new Size(520, 430);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

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

        var painel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20)
        };

        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 2
        };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        void Add(string label, Control c)
        {
            var linha = grid.RowCount++;
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            grid.Controls.Add(new Label { Text = label, AutoSize = true, Margin = new Padding(0, 8, 8, 0) }, 0, linha);
            c.Dock = DockStyle.Top;
            grid.Controls.Add(c, 1, linha);
        }

        Add("Material:", _cmbMaterial);
        Add("Quantidade:", _numQuantidade);
        Add("Nome:", _txtNome);

        var identificacaoPanel = new FlowLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Dock = DockStyle.Top,
            WrapContents = false
        };
        identificacaoPanel.Controls.Add(_rbCpf);
        identificacaoPanel.Controls.Add(_rbMatricula);
        Add("Identificação:", identificacaoPanel);
        Add("CPF/Matrícula:", _txtIdentificacao);
        Add("Setor:", _cmbSetor);

        var btnSalvar = new Button { Text = "Salvar", DialogResult = DialogResult.OK, Width = 90 };
        var btnCancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Width = 90 };
        var barraAcoes = ThemeHelper.CriarBarraAcoesInferior();
        barraAcoes.Controls.Add(btnCancelar);
        barraAcoes.Controls.Add(btnSalvar);

        painel.Controls.Add(barraAcoes);
        painel.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 12 });
        painel.Controls.Add(grid);
        Controls.Add(painel);

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
