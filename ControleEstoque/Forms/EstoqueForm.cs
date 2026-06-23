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

    private List<Material> _materiais = new();

    public EstoqueForm()
    {
        ThemeHelper.ConfigurarFormulario(this, "Consulta de Estoque", 950, 650);
        Controls.Add(ThemeHelper.CriarCabecalho());
        MontarInterface();
        CarregarMateriais();
    }

    private void MontarInterface()
    {
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 90, 20, 20) };

        var lblFiltro = new Label { Text = "Filtrar material:", AutoSize = true, Location = new Point(0, 0) };
        _txtFiltro.Location = new Point(0, 22);
        _txtFiltro.TextChanged += (_, _) => Filtrar();

        _lstMateriais.Location = new Point(0, 55);
        _lstMateriais.SelectedIndexChanged += (_, _) => ExibirDetalhes();

        var grpDetalhes = new GroupBox
        {
            Text = "Detalhes do Material",
            Location = new Point(380, 0),
            Size = new Size(520, 180)
        };

        _lblQuantidade.Location = new Point(15, 30);
        _lblLocalizacao.Location = new Point(15, 55);
        _lblPrecoUnitario.Location = new Point(15, 80);
        _lblPrecoTotal.Location = new Point(15, 105);
        grpDetalhes.Controls.AddRange(new Control[] { _lblQuantidade, _lblLocalizacao, _lblPrecoUnitario, _lblPrecoTotal });

        var grpSaidas = new GroupBox
        {
            Text = "Últimas Saídas",
            Location = new Point(0, 270),
            Size = new Size(900, 250),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
        };

        _gridSaidas.Dock = DockStyle.Fill;
        _gridSaidas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _gridSaidas.ReadOnly = true;
        _gridSaidas.AllowUserToAddRows = false;
        grpSaidas.Controls.Add(_gridSaidas);

        var btnFechar = ThemeHelper.CriarBotao("Fechar", 120, 35);
        btnFechar.Location = new Point(780, 530);
        btnFechar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnFechar.BackColor = Color.Gray;
        btnFechar.Click += (_, _) => Close();

        panel.Controls.AddRange(new Control[] { lblFiltro, _txtFiltro, _lstMateriais, grpDetalhes, grpSaidas, btnFechar });
        Controls.Add(panel);
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
