namespace ControleEstoque.Models;

public class RelatorioResultado
{
    public string Titulo { get; set; } = string.Empty;
    public string Subtitulo { get; set; } = string.Empty;
    public List<RelatorioItem> Itens { get; set; } = new();
    public decimal TotalGeral { get; set; }
}
