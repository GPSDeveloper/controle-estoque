using ControleEstoque.Data;
using ControleEstoque.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ControleEstoque.Services;

public class RelatorioService
{
    public RelatorioResultado GerarMensal(int setorId, int mes, int ano)
    {
        using var context = DbContextFactory.Create();
        var setor = context.Setores.Find(setorId);
        var nomeSetor = setor?.Nome ?? "Setor";

        var inicio = new DateTime(ano, mes, 1, 0, 0, 0, DateTimeKind.Utc);
        var fim = inicio.AddMonths(1);

        var saidas = context.Saidas
            .Include(s => s.Material)
            .Where(s => s.SetorId == setorId && s.DataHora >= inicio && s.DataHora < fim)
            .ToList();

        var itens = saidas
            .GroupBy(s => s.Material.Nome)
            .Select(g =>
            {
                var qtd = g.Sum(s => s.Quantidade);
                var preco = g.First().Material.PrecoUnitario;
                return new RelatorioItem
                {
                    MaterialNome = g.Key,
                    Quantidade = qtd,
                    PrecoUnitario = preco,
                    PrecoTotal = qtd * preco
                };
            })
            .OrderBy(i => i.MaterialNome)
            .ToList();

        var cultura = CultureInfo.GetCultureInfo("pt-BR");
        return new RelatorioResultado
        {
            Titulo = "Relatório Mensal de Saídas",
            Subtitulo = $"Setor: {nomeSetor} | Mês: {cultura.DateTimeFormat.GetMonthName(mes)}/{ano}",
            Itens = itens,
            TotalGeral = itens.Sum(i => i.PrecoTotal)
        };
    }

    public RelatorioResultado GerarAnual(int setorId, int ano)
    {
        using var context = DbContextFactory.Create();
        var setor = context.Setores.Find(setorId);
        var nomeSetor = setor?.Nome ?? "Setor";

        var inicio = new DateTime(ano, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var fim = inicio.AddYears(1);

        var saidas = context.Saidas
            .Include(s => s.Material)
            .Where(s => s.SetorId == setorId && s.DataHora >= inicio && s.DataHora < fim)
            .ToList();

        var itens = saidas
            .GroupBy(s => s.Material.Nome)
            .Select(g =>
            {
                var qtd = g.Sum(s => s.Quantidade);
                var preco = g.First().Material.PrecoUnitario;
                return new RelatorioItem
                {
                    MaterialNome = g.Key,
                    Quantidade = qtd,
                    PrecoUnitario = preco,
                    PrecoTotal = qtd * preco
                };
            })
            .OrderBy(i => i.MaterialNome)
            .ToList();

        return new RelatorioResultado
        {
            Titulo = "Relatório Anual de Saídas",
            Subtitulo = $"Setor: {nomeSetor} | Ano: {ano}",
            Itens = itens,
            TotalGeral = itens.Sum(i => i.PrecoTotal)
        };
    }

    public RelatorioResultado GerarInventario()
    {
        using var context = DbContextFactory.Create();
        var materiais = context.Materiais.OrderBy(m => m.Nome).ToList();

        var itens = materiais.Select(m => new RelatorioItem
        {
            MaterialNome = m.Nome,
            Quantidade = m.Quantidade,
            PrecoUnitario = m.PrecoUnitario,
            PrecoTotal = m.PrecoTotal,
            Localizacao = m.Localizacao
        }).ToList();

        return new RelatorioResultado
        {
            Titulo = "Relatório de Inventário",
            Subtitulo = $"Data: {DateTime.Now:dd/MM/yyyy HH:mm}",
            Itens = itens,
            TotalGeral = itens.Sum(i => i.PrecoTotal)
        };
    }
}
