using ControleEstoque.Data;
using ControleEstoque.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleEstoque.Services;

public class SaidaService
{
    public (bool Sucesso, string Mensagem) Registrar(
        int materialId,
        decimal quantidade,
        string nomeRetirante,
        TipoIdentificacao tipoIdentificacao,
        string identificacao,
        int setorId)
    {
        nomeRetirante = nomeRetirante.Trim();
        identificacao = identificacao.Trim();

        if (quantidade <= 0)
            return (false, "A quantidade deve ser maior que zero.");
        if (string.IsNullOrWhiteSpace(nomeRetirante))
            return (false, "Informe o nome de quem retirou o material.");
        if (string.IsNullOrWhiteSpace(identificacao))
            return (false, "Informe o CPF ou a matrícula.");

        using var context = DbContextFactory.Create();
        using var transaction = context.Database.BeginTransaction();

        try
        {
            var material = context.Materiais.Find(materialId);
            if (material == null)
                return (false, "Material não encontrado.");

            if (material.Quantidade < quantidade)
                return (false, $"Quantidade insuficiente. Disponível: {material.Quantidade:N2}");

            var setor = context.Setores.Find(setorId);
            if (setor == null)
                return (false, "Setor não encontrado.");

            material.Quantidade -= quantidade;
            material.AtualizadoEm = DateTime.UtcNow;

            context.Saidas.Add(new Saida
            {
                MaterialId = materialId,
                Quantidade = quantidade,
                NomeRetirante = nomeRetirante,
                TipoIdentificacao = tipoIdentificacao,
                Identificacao = identificacao,
                SetorId = setorId,
                DataHora = DateTime.UtcNow
            });

            context.SaveChanges();
            transaction.Commit();
            return (true, "Saída registrada com sucesso.");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return (false, $"Erro ao registrar saída: {ex.Message}");
        }
    }

    public List<SaidaHistoricoItem> ListarHistorico()
    {
        using var context = DbContextFactory.Create();
        return context.Saidas
            .Include(s => s.Material)
            .Include(s => s.Setor)
            .OrderByDescending(s => s.DataHora)
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

    public (bool Sucesso, string Mensagem) Excluir(int saidaId)
    {
        using var context = DbContextFactory.Create();
        using var transaction = context.Database.BeginTransaction();

        try
        {
            var saida = context.Saidas.Include(s => s.Material).FirstOrDefault(s => s.Id == saidaId);
            if (saida == null)
                return (false, "Saída não encontrada.");

            saida.Material.Quantidade += saida.Quantidade;
            saida.Material.AtualizadoEm = DateTime.UtcNow;

            context.Saidas.Remove(saida);
            context.SaveChanges();
            transaction.Commit();
            return (true, "Saída excluída e estoque atualizado.");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return (false, $"Erro ao excluir saída: {ex.Message}");
        }
    }

    public (bool Sucesso, string Mensagem) Editar(
        int saidaId,
        int materialId,
        decimal quantidade,
        string nomeRetirante,
        TipoIdentificacao tipoIdentificacao,
        string identificacao,
        int setorId)
    {
        nomeRetirante = nomeRetirante.Trim();
        identificacao = identificacao.Trim();

        if (quantidade <= 0)
            return (false, "A quantidade deve ser maior que zero.");
        if (string.IsNullOrWhiteSpace(nomeRetirante))
            return (false, "Informe o nome de quem retirou o material.");
        if (string.IsNullOrWhiteSpace(identificacao))
            return (false, "Informe o CPF ou a matrícula.");

        using var context = DbContextFactory.Create();
        using var transaction = context.Database.BeginTransaction();

        try
        {
            var saida = context.Saidas.Include(s => s.Material).FirstOrDefault(s => s.Id == saidaId);
            if (saida == null)
                return (false, "Saída não encontrada.");

            var materialAntigo = saida.Material;
            var quantidadeAntiga = saida.Quantidade;
            var materialIdAntigo = saida.MaterialId;

            if (materialId != materialIdAntigo)
            {
                materialAntigo.Quantidade += quantidadeAntiga;
                materialAntigo.AtualizadoEm = DateTime.UtcNow;

                var materialNovo = context.Materiais.Find(materialId);
                if (materialNovo == null)
                    return (false, "Material não encontrado.");

                if (materialNovo.Quantidade < quantidade)
                    return (false, $"Quantidade insuficiente no material selecionado. Disponível: {materialNovo.Quantidade:N2}");

                materialNovo.Quantidade -= quantidade;
                materialNovo.AtualizadoEm = DateTime.UtcNow;
            }
            else
            {
                var diferenca = quantidade - quantidadeAntiga;
                if (diferenca > 0 && materialAntigo.Quantidade < diferenca)
                    return (false, $"Quantidade insuficiente. Disponível: {materialAntigo.Quantidade:N2}");

                materialAntigo.Quantidade -= diferenca;
                materialAntigo.AtualizadoEm = DateTime.UtcNow;
            }

            var setor = context.Setores.Find(setorId);
            if (setor == null)
                return (false, "Setor não encontrado.");

            saida.MaterialId = materialId;
            saida.Quantidade = quantidade;
            saida.NomeRetirante = nomeRetirante;
            saida.TipoIdentificacao = tipoIdentificacao;
            saida.Identificacao = identificacao;
            saida.SetorId = setorId;

            context.SaveChanges();
            transaction.Commit();
            return (true, "Saída atualizada com sucesso.");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return (false, $"Erro ao editar saída: {ex.Message}");
        }
    }
}
