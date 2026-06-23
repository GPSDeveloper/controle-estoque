using ControleEstoque.Models;
using ControleEstoque.Services;
using ControleEstoque.UI;

namespace ControleEstoque.Forms;

public class SaidaMaterialForm : Form
{
    private readonly SaidaService _saidaService = new();
    private readonly MaterialService _materialService = new();
    private readonly SetorService _setorService = new();

    private readonly ComboBox _cmbMaterial = new() { Width = 400, DropDownStyle = ComboBoxStyle.DropDown };
    private readonly Label _lblDisponivel = new() { AutoSize = true, ForeColor = Color.DarkGreen };
    private readonly NumericUpDown _numQuantidade = new() { Width = 150, DecimalPlaces = 2, Minimum = 0.01m, Maximum = 999999, Value = 1 };
    private readonly TextBox _txtNome = new() { Width = 350 };
    private readonly RadioButton _rbCpf = new() { Text = "CPF", Checked = true, AutoSize = true };
    private readonly RadioButton _rbMatricula = new() { Text = "Matrícula", AutoSize = true };
    private readonly TextBox _txtIdentificacao = new() { Width = 250 };
    private readonly ComboBox _cmbSetor = new() { Width = 350, DropDownStyle = ComboBoxStyle.DropDownList };

    private List<Material> _materiais = new();

    public SaidaMaterialForm()
    {
        ThemeHelper.ConfigurarFormulario(this, "Saída de Material");
        Controls.Add(ThemeHelper.CriarCabecalho());
        MontarInterface();
        CarregarDados();
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

        AddField("Material (digite para buscar):", _cmbMaterial);
        _cmbMaterial.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        _cmbMaterial.AutoCompleteSource = AutoCompleteSource.ListItems;
        _cmbMaterial.SelectedIndexChanged += (_, _) => AtualizarDisponivel();
        _cmbMaterial.TextUpdate += (_, _) => FiltrarMateriais();

        _lblDisponivel.Location = new Point(0, y);
        panel.Controls.Add(_lblDisponivel);
        y += 25;

        AddField("Quantidade:", _numQuantidade);
        AddField("Nome de quem retirou:", _txtNome);

        panel.Controls.Add(new Label { Text = "Identificação:", AutoSize = true, Location = new Point(0, y) });
        _rbCpf.Location = new Point(0, y + 22);
        _rbMatricula.Location = new Point(80, y + 22);
        panel.Controls.Add(_rbCpf);
        panel.Controls.Add(_rbMatricula);
        y += 50;

        AddField(_rbCpf.Checked ? "CPF:" : "Matrícula:", _txtIdentificacao);
        _rbCpf.CheckedChanged += (_, _) => AtualizarLabelIdentificacao();
        _rbMatricula.CheckedChanged += (_, _) => AtualizarLabelIdentificacao();

        AddField("Setor:", _cmbSetor);

        var btnRetirar = ThemeHelper.CriarBotao("Retirar", 150, 40);
        btnRetirar.Location = new Point(0, y);
        btnRetirar.Click += (_, _) => Retirar();

        var btnFechar = ThemeHelper.CriarBotao("Fechar", 120, 40);
        btnFechar.Location = new Point(160, y);
        btnFechar.BackColor = Color.Gray;
        btnFechar.Click += (_, _) => Close();

        panel.Controls.AddRange(new Control[] { btnRetirar, btnFechar });
        Controls.Add(panel);
    }

    private void AtualizarLabelIdentificacao()
    {
        // Label is static in this layout; identification field is shared
    }

    private void CarregarDados()
    {
        _materiais = _materialService.ListarTodos();
        _cmbMaterial.Items.Clear();
        foreach (var m in _materiais)
            _cmbMaterial.Items.Add(m.Nome);

        var setores = _setorService.ListarTodos();
        _cmbSetor.DataSource = setores;
        _cmbSetor.DisplayMember = "Nome";
        _cmbSetor.ValueMember = "Id";
    }

    private void FiltrarMateriais()
    {
        var filtro = _cmbMaterial.Text.Trim().ToLower();
        var filtrados = string.IsNullOrEmpty(filtro)
            ? _materiais
            : _materiais.Where(m => m.Nome.ToLower().Contains(filtro)).ToList();

        _cmbMaterial.Items.Clear();
        foreach (var m in filtrados)
            _cmbMaterial.Items.Add(m.Nome);
        _cmbMaterial.DroppedDown = true;
        _cmbMaterial.Text = filtro;
        _cmbMaterial.SelectionStart = filtro.Length;
    }

    private Material? ObterMaterialSelecionado()
    {
        var nome = _cmbMaterial.Text.Trim();
        return _materiais.FirstOrDefault(m => m.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
    }

    private void AtualizarDisponivel()
    {
        var material = ObterMaterialSelecionado();
        _lblDisponivel.Text = material != null
            ? $"Disponível em estoque: {material.Quantidade:N2}"
            : "";
    }

    private void Retirar()
    {
        var material = ObterMaterialSelecionado();
        if (material == null)
        {
            MessageBox.Show("Selecione um material válido.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_cmbSetor.SelectedValue is not int setorId)
        {
            MessageBox.Show("Selecione um setor.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var tipo = _rbCpf.Checked ? TipoIdentificacao.CPF : TipoIdentificacao.Matricula;
        var (sucesso, mensagem) = _saidaService.Registrar(
            material.Id,
            _numQuantidade.Value,
            _txtNome.Text,
            tipo,
            _txtIdentificacao.Text,
            setorId);

        MessageBox.Show(mensagem, sucesso ? "Sucesso" : "Atenção", MessageBoxButtons.OK,
            sucesso ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

        if (sucesso)
        {
            _materiais = _materialService.ListarTodos();
            _txtNome.Clear();
            _txtIdentificacao.Clear();
            _numQuantidade.Value = 1;
            AtualizarDisponivel();
        }
    }
}
