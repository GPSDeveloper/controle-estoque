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
    private readonly Label _lblIdentificacao = new() { Text = "CPF:", AutoSize = true };
    private Panel _conteudo = null!;

    private List<Material> _materiais = new();

    public SaidaMaterialForm()
    {
        ThemeHelper.ConfigurarFormulario(this, "Saída de Material");
        _conteudo = ThemeHelper.CriarConteudoPrincipal(this, "Saída de Material", 860, new Padding(0));
        MontarInterface();
        CarregarDados();
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
        formGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));
        formGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        void AddField(Control label, Control control)
        {
            var linha = formGrid.RowCount++;
            formGrid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            label.Margin = new Padding(0, 8, 8, 0);
            control.Dock = DockStyle.Top;
            formGrid.Controls.Add(label, 0, linha);
            formGrid.Controls.Add(control, 1, linha);
        }

        AddField(new Label { Text = "Material (digite para buscar):", AutoSize = true }, _cmbMaterial);
        _cmbMaterial.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        _cmbMaterial.AutoCompleteSource = AutoCompleteSource.ListItems;
        _cmbMaterial.SelectedIndexChanged += (_, _) => AtualizarDisponivel();
        _cmbMaterial.TextUpdate += (_, _) => FiltrarMateriais();

        var linhaDisponivel = formGrid.RowCount++;
        formGrid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        formGrid.Controls.Add(new Label(), 0, linhaDisponivel);
        formGrid.Controls.Add(_lblDisponivel, 1, linhaDisponivel);

        AddField(new Label { Text = "Quantidade:", AutoSize = true }, _numQuantidade);
        AddField(new Label { Text = "Nome de quem retirou:", AutoSize = true }, _txtNome);

        var identificacaoPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = new Padding(0)
        };
        identificacaoPanel.Controls.Add(_rbCpf);
        identificacaoPanel.Controls.Add(_rbMatricula);
        AddField(new Label { Text = "Tipo de identificação:", AutoSize = true }, identificacaoPanel);

        AddField(_lblIdentificacao, _txtIdentificacao);
        _rbCpf.CheckedChanged += (_, _) => AtualizarLabelIdentificacao();
        _rbMatricula.CheckedChanged += (_, _) => AtualizarLabelIdentificacao();

        AddField(new Label { Text = "Setor:", AutoSize = true }, _cmbSetor);

        var btnRetirar = ThemeHelper.CriarBotao("Retirar", 150, 40);
        btnRetirar.Click += (_, _) => Retirar();

        var btnFechar = ThemeHelper.CriarBotao("Fechar", 120, 40);
        btnFechar.BackColor = Color.Gray;
        btnFechar.Click += (_, _) => Close();

        var barraAcoes = ThemeHelper.CriarBarraAcoesInferior();
        barraAcoes.Controls.Add(btnFechar);
        barraAcoes.Controls.Add(btnRetirar);

        _conteudo.Controls.Add(barraAcoes);
        _conteudo.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 12 });
        _conteudo.Controls.Add(formGrid);
    }

    private void AtualizarLabelIdentificacao()
    {
        _lblIdentificacao.Text = _rbCpf.Checked ? "CPF:" : "Matrícula:";
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
        var filtroOriginal = _cmbMaterial.Text.Trim();
        var filtro = filtroOriginal.ToLower();
        var filtrados = string.IsNullOrEmpty(filtro)
            ? _materiais
            : _materiais.Where(m => m.Nome.ToLower().Contains(filtro)).ToList();

        _cmbMaterial.Items.Clear();
        foreach (var m in filtrados)
            _cmbMaterial.Items.Add(m.Nome);
        _cmbMaterial.DroppedDown = true;
        _cmbMaterial.Text = filtroOriginal;
        _cmbMaterial.SelectionStart = filtroOriginal.Length;
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
        OperationFeedbackHelper.ExecutarComRetryConcorrencia(
            () => _saidaService.Registrar(
                material.Id,
                _numQuantidade.Value,
                _txtNome.Text,
                tipo,
                _txtIdentificacao.Text,
                setorId),
            onSuccess: () =>
            {
                _materiais = _materialService.ListarTodos();
                _txtNome.Clear();
                _txtIdentificacao.Clear();
                _numQuantidade.Value = 1;
                AtualizarDisponivel();
            });
    }
}
