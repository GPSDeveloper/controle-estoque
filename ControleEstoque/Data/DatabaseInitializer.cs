using ControleEstoque.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleEstoque.Data;

public static class DatabaseInitializer
{
    private const string AdminLogin = "almoxarifado12";
    private const string AdminPassword = "estoque123";

    public static void Initialize(AppDbContext context)
    {
        context.Database.Migrate();

        if (!context.Usuarios.Any(u => u.Login == AdminLogin))
        {
            context.Usuarios.Add(new Usuario
            {
                Login = AdminLogin,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(AdminPassword)
            });
            context.SaveChanges();
        }
    }
}
