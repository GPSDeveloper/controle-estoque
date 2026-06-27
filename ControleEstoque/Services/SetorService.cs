using ControleEstoque.Data;
using ControleEstoque.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleEstoque.Services;

public class SetorService
{
    public List<Setor> ListarTodos()
    {
        using var context = DbContextFactory.Create();
        return context.Setores.OrderBy(s => s.Nome).ToList();
    }

    public (bool Sucesso, string Mensagem) Adicionar(string nome)
    {
        nome = nome.Trim();
        if (string.IsNullOrWhiteSpace(nome))
            return (false, "Informe o nome do setor.");

        using var context = DbContextFactory.Create();
        if (context.Setores.Any(s => s.Nome.ToLower() == nome.ToLower()))
            return (false, "Já existe um setor com este nome.");

        context.Setores.Add(new Setor { Nome = nome, CriadoEm = DateTime.UtcNow });
        context.SaveChanges();
        return (true, "Setor adicionado com sucesso.");
    }

    public (bool Sucesso, string Mensagem) Editar(int id, string novoNome)
    {
        novoNome = novoNome.Trim();
        if (string.IsNullOrWhiteSpace(novoNome))
            return (false, "Informe o nome do setor.");

        using var context = DbContextFactory.Create();
        var setor = context.Setores.Find(id);
        if (setor == null)
            return (false, "Setor não encontrado.");

        if (context.Setores.Any(s => s.Id != id && s.Nome.ToLower() == novoNome.ToLower()))
            return (false, "Já existe um setor com este nome.");

        setor.Nome = novoNome;
        context.SaveChanges();
        return (true, "Setor atualizado com sucesso.");
    }

    public (bool Sucesso, string Mensagem) Excluir(int id)
    {
        using var context = DbContextFactory.Create();
        var setor = context.Setores.Include(s => s.Saidas).FirstOrDefault(s => s.Id == id);
        if (setor == null)
            return (false, "Setor não encontrado.");

        if (setor.Saidas.Any())
            return (false, "Não é possível excluir: existem saídas vinculadas a este setor.");

        context.Setores.Remove(setor);
        context.SaveChanges();
        return (true, "Setor excluído com sucesso.");
    }
}
