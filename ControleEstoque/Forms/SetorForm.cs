using ControleEstoque.Services;
using ControleEstoque.UI;

namespace ControleEstoque.Forms;

public class SetorForm : Form
{
    private readonly SetorService _service = new();
    private readonly DataGridView _grid = new();
    private readonly TextBox _txtNome = new() { Width = 300 };
    private Panel _conteudo = null!;

    public SetorForm()
    {
        ThemeHelper.ConfigurarFormulario(this, "Gerenciar Setores");
        _conteudo = ThemeHelper.CriarConteudoPrincipal(this, "Adicionar Setor", 1100, new Padding(0));
        MontarInterface();
        CarregarDados();
    }

    private void MontarInterface()
    {
        var topo = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 3,
            Margin = new Padding(0)
        };
        topo.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        topo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        topo.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        var lbl = new Label { Text = "Nome do setor:", AutoSize = true, Margin = new Padding(0, 10, 8, 0) };
        _txtNome.Dock = DockStyle.Top;
        var btnAdicionar = ThemeHelper.CriarBotao("Adicionar", 120, 35);
        btnAdicionar.Margin = new Padding(8, 0, 0, 0);
        btnAdicionar.Click += (_, _) => Adicionar();

        topo.Controls.Add(lbl, 0, 0);
        topo.Controls.Add(_txtNome, 1, 0);
        topo.Controls.Add(btnAdicionar, 2, 0);

        _grid.Dock = DockStyle.Fill;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.MultiSelect = false;

        var btnEditar = ThemeHelper.CriarBotao("Editar", 120, 35);
        btnEditar.Click += (_, _) => Editar();

        var btnExcluir = ThemeHelper.CriarBotao("Excluir", 120, 35);
        btnExcluir.BackColor = Color.DarkRed;
        btnExcluir.Click += (_, _) => Excluir();

        var btnFechar = ThemeHelper.CriarBotao("Fechar", 120, 35);
        btnFechar.BackColor = Color.Gray;
        btnFechar.Click += (_, _) => Close();

        var barraAcoes = ThemeHelper.CriarBarraAcoesInferior();
        barraAcoes.Controls.Add(btnFechar);
        barraAcoes.Controls.Add(btnExcluir);
        barraAcoes.Controls.Add(btnEditar);

        var areaGrid = new Panel { Dock = DockStyle.Fill };
        areaGrid.Controls.Add(_grid);
        areaGrid.Controls.Add(topo);

        _conteudo.Controls.Add(barraAcoes);
        _conteudo.Controls.Add(areaGrid);
    }

    private void CarregarDados()
    {
        var setores = _service.ListarTodos();
        _grid.DataSource = setores.Select(s => new { s.Id, s.Nome, CriadoEm = s.CriadoEm.ToLocalTime().ToString("dd/MM/yyyy") }).ToList();
        if (_grid.Columns.Contains("Id"))
            _grid.Columns["Id"]!.Visible = false;
    }

    private int? ObterIdSelecionado()
    {
        if (_grid.CurrentRow?.Cells["Id"].Value is int id)
            return id;
        return null;
    }

    private void Adicionar()
    {
        OperationFeedbackHelper.ExecutarComRetryConcorrencia(
            () => _service.Adicionar(_txtNome.Text),
            onSuccess: () =>
            {
                _txtNome.Clear();
                CarregarDados();
            });
    }

    private void Editar()
    {
        var id = ObterIdSelecionado();
        if (!id.HasValue)
        {
            MessageBox.Show("Selecione um setor.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var nomeAtual = _grid.CurrentRow?.Cells["Nome"].Value?.ToString() ?? "";
        var novoNome = InputDialogForm.Mostrar("Editar Setor", "Novo nome do setor:", nomeAtual);
        if (string.IsNullOrWhiteSpace(novoNome) || novoNome == nomeAtual)
            return;

        OperationFeedbackHelper.ExecutarComRetryConcorrencia(
            () => _service.Editar(id.Value, novoNome),
            onSuccess: CarregarDados);
    }

    private void Excluir()
    {
        var id = ObterIdSelecionado();
        if (!id.HasValue)
        {
            MessageBox.Show("Selecione um setor.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (MessageBox.Show("Deseja excluir o setor selecionado?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        OperationFeedbackHelper.ExecutarComRetryConcorrencia(
            () => _service.Excluir(id.Value),
            onSuccess: CarregarDados);
    }
}
