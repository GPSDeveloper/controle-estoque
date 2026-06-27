using ControleEstoque.Data;
using ControleEstoque.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleEstoque.Services;

public class MaterialService
{
    public List<Material> ListarTodos()
    {
        using var context = DbContextFactory.Create();
        return context.Materiais.OrderBy(m => m.Nome).ToList();
    }

    public List<Material> BuscarPorNome(string filtro)
    {
        using var context = DbContextFactory.Create();
        filtro = filtro.Trim().ToLower();
        return context.Materiais
            .Where(m => m.Nome.ToLower().Contains(filtro))
            .OrderBy(m => m.Nome)
            .ToList();
    }

    public Material? ObterPorId(int id)
    {
        using var context = DbContextFactory.Create();
        return context.Materiais.Find(id);
    }

    public (bool Sucesso, string Mensagem) Adicionar(
        string nome,
        decimal quantidade,
        decimal precoUnitario,
        string localizacao,
        bool possuiValidade,
        DateTime? dataValidade)
    {
        nome = nome.Trim();
        localizacao = localizacao.Trim();

        if (string.IsNullOrWhiteSpace(nome))
            return (false, "Informe o nome do material.");
        if (quantidade <= 0)
            return (false, "A quantidade deve ser maior que zero.");
        if (precoUnitario < 0)
            return (false, "O preço unitário não pode ser negativo.");
        if (possuiValidade && !dataValidade.HasValue)
            return (false, "Informe a data de validade.");

        using var context = DbContextFactory.Create();
        var agora = DateTime.UtcNow;
        context.Materiais.Add(new Material
        {
            Nome = nome,
            Quantidade = quantidade,
            PrecoUnitario = precoUnitario,
            Localizacao = localizacao,
            PossuiValidade = possuiValidade,
            DataValidade = possuiValidade ? DateTime.SpecifyKind(dataValidade!.Value.Date, DateTimeKind.Utc) : null,
            CriadoEm = agora,
            AtualizadoEm = agora
        });
        context.SaveChanges();
        return (true, "Material adicionado ao estoque com sucesso.");
    }

    public List<SaidaHistoricoItem> ObterUltimasSaidas(int materialId, int quantidade = 10)
    {
        using var context = DbContextFactory.Create();
        return context.Saidas
            .Include(s => s.Material)
            .Include(s => s.Setor)
            .Where(s => s.MaterialId == materialId)
            .OrderByDescending(s => s.DataHora)
            .Take(quantidade)
            .Select(s => new SaidaHistoricoItem
            {
                Id = s.Id,
                DataHora = s.DataHora,
                MaterialNome = s.Material.Nome,
                MaterialId = s.MaterialId,
                Quantidade = s.Quantidade,
                NomeRetirante = s.NomeRetirante,
                TipoIdentificacao = s.TipoIdentificacao == TipoIdentificacao.CPF ? "CPF" : "Matrícula",
                Identificacao = s.Identificacao,
                SetorNome = s.Setor.Nome,
                SetorId = s.SetorId
            })
            .ToList();
    }
}
