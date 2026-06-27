using ControleEstoque.Models;
using ControleEstoque.Services;
using ControleEstoque.UI;

namespace ControleEstoque.Forms;

public class EstoqueForm : Form
{
    private readonly MaterialService _service = new();
    private readonly TextBox _txtFiltro = new() { Width = 350 };
    private readonly ListBox _lstMateriais = new() { Width = 350, Height = 200 };
    private readonly Label _lblQuantidade = new() { AutoSize = true };
    private readonly Label _lblLocalizacao = new() { AutoSize = true };
    private readonly Label _lblPrecoUnitario = new() { AutoSize = true };
    private readonly Label _lblPrecoTotal = new() { AutoSize = true };
    private readonly DataGridView _gridSaidas = new();
    private Panel _conteudo = null!;

    private List<Material> _materiais = new();

    public EstoqueForm()
    {
        ThemeHelper.ConfigurarFormulario(this, "Consulta de Estoque", 950, 650);
        _conteudo = ThemeHelper.CriarConteudoPrincipal(this, 1200, new Padding(0));
        MontarInterface();
        CarregarMateriais();
    }

    private void MontarInterface()
    {
        var topo = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 240,
            ColumnCount = 2,
            Margin = new Padding(0)
        };
        topo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
        topo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));

        var painelBusca = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1
        };
        painelBusca.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        painelBusca.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        painelBusca.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var lblFiltro = new Label { Text = "Filtrar material:", AutoSize = true, Margin = new Padding(0, 0, 0, 4) };
        _txtFiltro.Dock = DockStyle.Top;
        _txtFiltro.TextChanged += (_, _) => Filtrar();
        _txtFiltro.Margin = new Padding(0, 0, 0, 8);

        _lstMateriais.Dock = DockStyle.Fill;
        _lstMateriais.SelectedIndexChanged += (_, _) => ExibirDetalhes();
        _lstMateriais.IntegralHeight = false;

        painelBusca.Controls.Add(lblFiltro);
        painelBusca.Controls.Add(_txtFiltro);
        painelBusca.Controls.Add(_lstMateriais);

        var grpDetalhes = new GroupBox
        {
            Text = "Detalhes do Material",
            Dock = DockStyle.Fill
        };

        var detalhesGrid = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            Padding = new Padding(12)
        };
        detalhesGrid.Controls.Add(_lblQuantidade);
        detalhesGrid.Controls.Add(_lblLocalizacao);
        detalhesGrid.Controls.Add(_lblPrecoUnitario);
        detalhesGrid.Controls.Add(_lblPrecoTotal);
        grpDetalhes.Controls.Add(detalhesGrid);

        topo.Controls.Add(painelBusca, 0, 0);
        topo.Controls.Add(grpDetalhes, 1, 0);

        var grpSaidas = new GroupBox
        {
            Text = "Últimas Saídas",
            Dock = DockStyle.Top,
            Height = 280
        };

        _gridSaidas.Dock = DockStyle.Fill;
        _gridSaidas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _gridSaidas.ReadOnly = true;
        _gridSaidas.AllowUserToAddRows = false;
        grpSaidas.Controls.Add(_gridSaidas);

        var btnFechar = ThemeHelper.CriarBotao("Fechar", 120, 35);
        btnFechar.BackColor = Color.Gray;
        btnFechar.Click += (_, _) => Close();

        var barraAcoes = ThemeHelper.CriarBarraAcoesInferior();
        barraAcoes.Controls.Add(btnFechar);

        _conteudo.Controls.Add(barraAcoes);
        _conteudo.Controls.Add(grpSaidas);
        _conteudo.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 12 });
        _conteudo.Controls.Add(topo);
    }

    private void CarregarMateriais()
    {
        _materiais = _service.ListarTodos();
        Filtrar();
    }

    private void Filtrar()
    {
        var filtro = _txtFiltro.Text.Trim().ToLower();
        var lista = string.IsNullOrEmpty(filtro)
            ? _materiais
            : _materiais.Where(m => m.Nome.ToLower().Contains(filtro)).ToList();

        _lstMateriais.Items.Clear();
        foreach (var m in lista)
            _lstMateriais.Items.Add(m.Nome);
    }

    private Material? ObterMaterialSelecionado()
    {
        if (_lstMateriais.SelectedItem is not string nome)
            return null;
        return _materiais.FirstOrDefault(m => m.Nome == nome);
    }

    private void ExibirDetalhes()
    {
        var material = ObterMaterialSelecionado();
        if (material == null)
        {
            _lblQuantidade.Text = "";
            _lblLocalizacao.Text = "";
            _lblPrecoUnitario.Text = "";
            _lblPrecoTotal.Text = "";
            _gridSaidas.DataSource = null;
            return;
        }

        _lblQuantidade.Text = $"Quantidade: {material.Quantidade:N2}";
        _lblLocalizacao.Text = $"Localização: {material.Localizacao}";
        _lblPrecoUnitario.Text = $"Preço unitário: {material.PrecoUnitario:C2}";
        _lblPrecoTotal.Text = $"Preço total: {material.PrecoTotal:C2}";

        var saidas = _service.ObterUltimasSaidas(material.Id);
        _gridSaidas.DataSource = saidas.Select(s => new
        {
            DataHora = s.DataHora.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
            s.Quantidade,
            s.NomeRetirante,
            Identificacao = $"{s.TipoIdentificacao}: {s.Identificacao}",
            s.SetorNome
        }).ToList();
    }
}
