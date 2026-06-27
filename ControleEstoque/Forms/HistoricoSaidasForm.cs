using ControleEstoque.Services;
using ControleEstoque.UI;

namespace ControleEstoque.Forms;

public class HistoricoSaidasForm : Form
{
    private readonly SaidaService _service = new();
    private readonly DataGridView _grid = new();
    private Panel _conteudo = null!;

    public HistoricoSaidasForm()
    {
        ThemeHelper.ConfigurarFormulario(this, "Histórico de Saídas", 1000, 600);
        _conteudo = ThemeHelper.CriarConteudoPrincipal(this, "Histórico de Saídas", 1200, new Padding(0));
        MontarInterface();
        CarregarDados();
    }

    private void MontarInterface()
    {
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

        var btnAtualizar = ThemeHelper.CriarBotao("Atualizar", 120, 35);
        btnAtualizar.Click += (_, _) => CarregarDados();

        var btnFechar = ThemeHelper.CriarBotao("Fechar", 120, 35);
        btnFechar.BackColor = Color.Gray;
        btnFechar.Click += (_, _) => Close();

        var barraAcoes = ThemeHelper.CriarBarraAcoesInferior();
        barraAcoes.Controls.Add(btnFechar);
        barraAcoes.Controls.Add(btnAtualizar);
        barraAcoes.Controls.Add(btnExcluir);
        barraAcoes.Controls.Add(btnEditar);

        var areaGrid = new Panel { Dock = DockStyle.Fill };
        areaGrid.Controls.Add(_grid);

        _conteudo.Controls.Add(barraAcoes);
        _conteudo.Controls.Add(areaGrid);
    }

    private void CarregarDados()
    {
        var historico = _service.ListarHistorico();
        _grid.DataSource = historico.Select(h => new
        {
            h.Id,
            DataHora = h.DataHora.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
            h.MaterialNome,
            h.Quantidade,
            h.NomeRetirante,
            Identificacao = $"{h.TipoIdentificacao}: {h.Identificacao}",
            h.SetorNome
        }).ToList();

        if (_grid.Columns.Contains("Id"))
            _grid.Columns["Id"]!.Visible = false;
    }

    private int? ObterIdSelecionado()
    {
        if (_grid.CurrentRow?.Cells["Id"].Value is int id)
            return id;
        return null;
    }

    private void Editar()
    {
        var id = ObterIdSelecionado();
        if (!id.HasValue)
        {
            MessageBox.Show("Selecione uma saída.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var historico = _service.ListarHistorico();
        var item = historico.FirstOrDefault(h => h.Id == id.Value);
        if (item == null) return;

        using var dialog = new EditarSaidaForm(item);
        if (dialog.ShowDialog() != DialogResult.OK)
            return;

        var dados = dialog.ObterDados();
        OperationFeedbackHelper.ExecutarComRetryConcorrencia(
            () => _service.Editar(
                id.Value,
                dados.MaterialId,
                dados.Quantidade,
                dados.Nome,
                dados.Tipo,
                dados.Identificacao,
                dados.SetorId),
            onSuccess: () =>
            {
                CarregarDados();
            });
    }

    private void Excluir()
    {
        var id = ObterIdSelecionado();
        if (!id.HasValue)
        {
            MessageBox.Show("Selecione uma saída.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (MessageBox.Show("Deseja excluir esta saída? O estoque será atualizado.", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        OperationFeedbackHelper.ExecutarComRetryConcorrencia(
            () => _service.Excluir(id.Value),
            onSuccess: () =>
            {
                CarregarDados();
            });
    }
}
