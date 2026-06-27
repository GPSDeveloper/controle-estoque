using ControleEstoque.Data;
using ControleEstoque.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace ControleEstoque.Services;

public class SaidaService
{
    private const int MaxRetryAttempts = 3;

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

        for (var attempt = 1; attempt <= MaxRetryAttempts; attempt++)
        {
            using var context = DbContextFactory.Create();
            using var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                var material = context.Materiais.AsNoTracking().FirstOrDefault(m => m.Id == materialId);
                if (material == null)
                    return (false, "Material não encontrado.");

                var setorExiste = context.Setores.AsNoTracking().Any(s => s.Id == setorId);
                if (!setorExiste)
                    return (false, "Setor não encontrado.");

                var agora = DateTime.UtcNow;
                var linhasAfetadas = context.Database.ExecuteSqlInterpolated(
                    $@"UPDATE ""Materiais""
                       SET ""Quantidade"" = ""Quantidade"" - {quantidade},
                           ""AtualizadoEm"" = {agora}
                       WHERE ""Id"" = {materialId}
                         AND ""Quantidade"" >= {quantidade};");

                if (linhasAfetadas == 0)
                {
                    var disponivel = context.Materiais
                        .AsNoTracking()
                        .Where(m => m.Id == materialId)
                        .Select(m => m.Quantidade)
                        .FirstOrDefault();
                    return (false, $"Quantidade insuficiente. Disponível: {disponivel:N2}");
                }

                context.Saidas.Add(new Saida
                {
                    MaterialId = materialId,
                    Quantidade = quantidade,
                    PrecoUnitarioNaSaida = material.PrecoUnitario,
                    NomeRetirante = nomeRetirante,
                    TipoIdentificacao = tipoIdentificacao,
                    Identificacao = identificacao,
                    SetorId = setorId,
                    DataHora = agora
                });

                context.SaveChanges();
                transaction.Commit();
                return (true, "Saída registrada com sucesso.");
            }
            catch (Exception ex) when (IsRetryableConcurrencyException(ex) && attempt < MaxRetryAttempts)
            {
                transaction.Rollback();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                if (IsRetryableConcurrencyException(ex))
                    return (false, "Conflito de concorrência no estoque. Tente novamente.");

                return (false, $"Erro ao registrar saída: {ex.Message}");
            }
        }

        return (false, "Conflito de concorrência no estoque. Tente novamente.");
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
        for (var attempt = 1; attempt <= MaxRetryAttempts; attempt++)
        {
            using var context = DbContextFactory.Create();
            using var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable);

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
            catch (Exception ex) when (IsRetryableConcurrencyException(ex) && attempt < MaxRetryAttempts)
            {
                transaction.Rollback();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                if (IsRetryableConcurrencyException(ex))
                    return (false, "Conflito de concorrência no estoque. Tente novamente.");

                return (false, $"Erro ao excluir saída: {ex.Message}");
            }
        }

        return (false, "Conflito de concorrência no estoque. Tente novamente.");
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

        for (var attempt = 1; attempt <= MaxRetryAttempts; attempt++)
        {
            using var context = DbContextFactory.Create();
            using var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable);

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
                    saida.PrecoUnitarioNaSaida = materialNovo.PrecoUnitario;
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
            catch (Exception ex) when (IsRetryableConcurrencyException(ex) && attempt < MaxRetryAttempts)
            {
                transaction.Rollback();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                if (IsRetryableConcurrencyException(ex))
                    return (false, "Conflito de concorrência no estoque. Tente novamente.");

                return (false, $"Erro ao editar saída: {ex.Message}");
            }
        }

        return (false, "Conflito de concorrência no estoque. Tente novamente.");
    }

    private static bool IsRetryableConcurrencyException(Exception ex)
    {
        if (ex is PostgresException pg &&
            (pg.SqlState == PostgresErrorCodes.SerializationFailure ||
             pg.SqlState == PostgresErrorCodes.DeadlockDetected))
        {
            return true;
        }

        if (ex is DbUpdateException dbUpdateEx && dbUpdateEx.InnerException != null)
            return IsRetryableConcurrencyException(dbUpdateEx.InnerException);

        return ex.InnerException != null && IsRetryableConcurrencyException(ex.InnerException);
    }
}
