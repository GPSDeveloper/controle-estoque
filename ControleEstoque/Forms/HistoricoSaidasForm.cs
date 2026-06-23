using ControleEstoque.Services;
using ControleEstoque.UI;

namespace ControleEstoque.Forms;

public class HistoricoSaidasForm : Form
{
    private readonly SaidaService _service = new();
    private readonly DataGridView _grid = new();

    public HistoricoSaidasForm()
    {
        ThemeHelper.ConfigurarFormulario(this, "Histórico de Saídas", 1000, 600);
        Controls.Add(ThemeHelper.CriarCabecalho());
        MontarInterface();
        CarregarDados();
    }

    private void MontarInterface()
    {
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 90, 20, 20) };

        _grid.Dock = DockStyle.Top;
        _grid.Height = 400;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.MultiSelect = false;

        var btnEditar = ThemeHelper.CriarBotao("Editar", 120, 35);
        btnEditar.Location = new Point(0, 420);
        btnEditar.Click += (_, _) => Editar();

        var btnExcluir = ThemeHelper.CriarBotao("Excluir", 120, 35);
        btnExcluir.Location = new Point(130, 420);
        btnExcluir.BackColor = Color.DarkRed;
        btnExcluir.Click += (_, _) => Excluir();

        var btnAtualizar = ThemeHelper.CriarBotao("Atualizar", 120, 35);
        btnAtualizar.Location = new Point(260, 420);
        btnAtualizar.Click += (_, _) => CarregarDados();

        var btnFechar = ThemeHelper.CriarBotao("Fechar", 120, 35);
        btnFechar.Location = new Point(820, 420);
        btnFechar.BackColor = Color.Gray;
        btnFechar.Click += (_, _) => Close();

        panel.Controls.AddRange(new Control[] { _grid, btnEditar, btnExcluir, btnAtualizar, btnFechar });
        Controls.Add(panel);
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
        var (sucesso, mensagem) = _service.Editar(
            id.Value,
            dados.MaterialId,
            dados.Quantidade,
            dados.Nome,
            dados.Tipo,
            dados.Identificacao,
            dados.SetorId);

        MessageBox.Show(mensagem, sucesso ? "Sucesso" : "Atenção", MessageBoxButtons.OK,
            sucesso ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        if (sucesso)
            CarregarDados();
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

        var (sucesso, mensagem) = _service.Excluir(id.Value);
        MessageBox.Show(mensagem, sucesso ? "Sucesso" : "Atenção", MessageBoxButtons.OK,
            sucesso ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        if (sucesso)
            CarregarDados();
    }
}
