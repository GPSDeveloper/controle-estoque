namespace ControleEstoque.Models;

public class RelatorioItem
{
    public string MaterialNome { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal PrecoTotal { get; set; }
    public string? Localizacao { get; set; }
}
