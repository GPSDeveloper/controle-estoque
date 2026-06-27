namespace ControleEstoque.Models;

public class SaidaHistoricoItem
{
    public int Id { get; set; }
    public DateTime DataHora { get; set; }
    public string MaterialNome { get; set; } = string.Empty;
    public int MaterialId { get; set; }
    public decimal Quantidade { get; set; }
    public string NomeRetirante { get; set; } = string.Empty;
    public string TipoIdentificacao { get; set; } = string.Empty;
    public string Identificacao { get; set; } = string.Empty;
    public string SetorNome { get; set; } = string.Empty;
    public int SetorId { get; set; }
}
