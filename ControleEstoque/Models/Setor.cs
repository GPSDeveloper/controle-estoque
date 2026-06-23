namespace ControleEstoque.Models;

public class Setor
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<Saida> Saidas { get; set; } = new List<Saida>();
}
