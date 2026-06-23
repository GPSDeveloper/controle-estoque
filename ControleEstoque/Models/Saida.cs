namespace ControleEstoque.Models;

public class Saida
{
    public int Id { get; set; }
    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;
    public decimal Quantidade { get; set; }
    public string NomeRetirante { get; set; } = string.Empty;
    public TipoIdentificacao TipoIdentificacao { get; set; }
    public string Identificacao { get; set; } = string.Empty;
    public int SetorId { get; set; }
    public Setor Setor { get; set; } = null!;
    public DateTime DataHora { get; set; } = DateTime.UtcNow;
}
