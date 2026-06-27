using ControleEstoque.Models;
using ControleEstoque.UI;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ControleEstoque.Reports;

public static class PdfReportGenerator
{
    static PdfReportGenerator()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public static void Gerar(RelatorioResultado relatorio, string caminhoArquivo, bool incluirLocalizacao = false)
    {
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(col =>
                {
                    col.Item().Text(ThemeHelper.Instituicao).Bold().FontSize(14).FontColor(Colors.Blue.Darken3);
                    col.Item().PaddingTop(5).Text(relatorio.Titulo).Bold().FontSize(12);
                    col.Item().Text(relatorio.Subtitulo).FontSize(10);
                    col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                });

                page.Content().PaddingTop(15).Table(table =>
                {
                    if (incluirLocalizacao)
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1.5f);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Material").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Qtd").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Localização").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Preço Unit.").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Preço Total").Bold();
                        });

                        foreach (var item in relatorio.Itens)
                        {
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(5).Text(item.MaterialNome);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(5).Text(item.Quantidade.ToString("N2"));
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(5).Text(item.Localizacao ?? "-");
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(5).Text(item.PrecoUnitario.ToString("C2"));
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(5).Text(item.PrecoTotal.ToString("C2"));
                        }
                    }
                    else
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1.5f);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Material").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Qtd").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Preço Unit.").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Preço Total").Bold();
                        });

                        foreach (var item in relatorio.Itens)
                        {
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(5).Text(item.MaterialNome);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(5).Text(item.Quantidade.ToString("N2"));
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(5).Text(item.PrecoUnitario.ToString("C2"));
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(5).Text(item.PrecoTotal.ToString("C2"));
                        }
                    }
                });

                page.Footer().AlignRight().Text(text =>
                {
                    text.Span("Total Geral: ").Bold();
                    text.Span(relatorio.TotalGeral.ToString("C2")).Bold();
                    text.Span($"  |  Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}");
                });
            });
        }).GeneratePdf(caminhoArquivo);
    }
}
