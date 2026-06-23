namespace ControleEstoque.Models;

public class Material
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public bool PossuiValidade { get; set; }
    public DateTime? DataValidade { get; set; }
    public string Localizacao { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<Saida> Saidas { get; set; } = new List<Saida>();

    public decimal PrecoTotal => Quantidade * PrecoUnitario;
}
