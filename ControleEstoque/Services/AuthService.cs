using ControleEstoque.Data;
using ControleEstoque.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleEstoque.Services;

public class AuthService
{
    public Usuario? Autenticar(string login, string senha)
    {
        using var context = DbContextFactory.Create();
        var usuario = context.Usuarios.FirstOrDefault(u => u.Login == login.Trim());

        if (usuario == null)
            return null;

        return BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash) ? usuario : null;
    }
}
