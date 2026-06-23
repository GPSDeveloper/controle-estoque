using ControleEstoque.Services;
using ControleEstoque.UI;

namespace ControleEstoque.Forms;

public class SetorForm : Form
{
    private readonly SetorService _service = new();
    private readonly DataGridView _grid = new();
    private readonly TextBox _txtNome = new() { Width = 300 };

    public SetorForm()
    {
        ThemeHelper.ConfigurarFormulario(this, "Gerenciar Setores");
        Controls.Add(ThemeHelper.CriarCabecalho());
        MontarInterface();
        CarregarDados();
    }

    private void MontarInterface()
    {
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 90, 20, 20) };

        var lbl = new Label { Text = "Nome do setor:", AutoSize = true, Location = new Point(0, 0) };
        _txtNome.Location = new Point(0, 25);

        var btnAdicionar = ThemeHelper.CriarBotao("Adicionar", 120, 35);
        btnAdicionar.Location = new Point(310, 23);
        btnAdicionar.Click += (_, _) => Adicionar();

        _grid.Location = new Point(0, 70);
        _grid.Size = new Size(820, 350);
        _grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.MultiSelect = false;

        var btnEditar = ThemeHelper.CriarBotao("Editar", 120, 35);
        btnEditar.Location = new Point(0, 430);
        btnEditar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnEditar.Click += (_, _) => Editar();

        var btnExcluir = ThemeHelper.CriarBotao("Excluir", 120, 35);
        btnExcluir.Location = new Point(130, 430);
        btnExcluir.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnExcluir.BackColor = Color.DarkRed;
        btnExcluir.Click += (_, _) => Excluir();

        var btnFechar = ThemeHelper.CriarBotao("Fechar", 120, 35);
        btnFechar.Location = new Point(700, 430);
        btnFechar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnFechar.BackColor = Color.Gray;
        btnFechar.Click += (_, _) => Close();

        panel.Controls.AddRange(new Control[] { lbl, _txtNome, btnAdicionar, _grid, btnEditar, btnExcluir, btnFechar });
        Controls.Add(panel);
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
