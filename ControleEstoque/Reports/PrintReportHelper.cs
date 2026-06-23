using ControleEstoque.Models;
using System.Drawing.Printing;

namespace ControleEstoque.Reports;

public class PrintReportHelper
{
    private readonly RelatorioResultado _relatorio;
    private readonly bool _incluirLocalizacao;
    private int _linhaAtual;

    public PrintReportHelper(RelatorioResultado relatorio, bool incluirLocalizacao = false)
    {
        _relatorio = relatorio;
        _incluirLocalizacao = incluirLocalizacao;
    }

    public void Imprimir()
    {
        var document = new PrintDocument();
        document.DocumentName = _relatorio.Titulo;
        document.PrintPage += Document_PrintPage;

        using var dialog = new PrintDialog { Document = document, UseEXDialog = true };
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            _linhaAtual = 0;
            document.Print();
        }
    }

    private void Document_PrintPage(object sender, PrintPageEventArgs e)
    {
        if (e.Graphics == null) return;

        var g = e.Graphics;
        var fonteTitulo = new Font("Segoe UI", 12, FontStyle.Bold);
        var fonteNormal = new Font("Segoe UI", 9);
        var fonteBold = new Font("Segoe UI", 9, FontStyle.Bold);
        float y = 40;
        float margem = 40;

        g.DrawString("Ministério da Saúde / SEMSRJ / Almoxarifado", fonteTitulo, Brushes.DarkBlue, margem, y);
        y += 30;
        g.DrawString(_relatorio.Titulo, fonteBold, Brushes.Black, margem, y);
        y += 20;
        g.DrawString(_relatorio.Subtitulo, fonteNormal, Brushes.Black, margem, y);
        y += 30;

        if (_linhaAtual == 0)
        {
            if (_incluirLocalizacao)
            {
                g.DrawString("Material", fonteBold, Brushes.Black, margem, y);
                g.DrawString("Qtd", fonteBold, Brushes.Black, margem + 200, y);
                g.DrawString("Localização", fonteBold, Brushes.Black, margem + 270, y);
                g.DrawString("Preço Unit.", fonteBold, Brushes.Black, margem + 420, y);
                g.DrawString("Preço Total", fonteBold, Brushes.Black, margem + 520, y);
            }
            else
            {
                g.DrawString("Material", fonteBold, Brushes.Black, margem, y);
                g.DrawString("Qtd", fonteBold, Brushes.Black, margem + 250, y);
                g.DrawString("Preço Unit.", fonteBold, Brushes.Black, margem + 320, y);
                g.DrawString("Preço Total", fonteBold, Brushes.Black, margem + 420, y);
            }
            y += 25;
            _linhaAtual = 0;
        }

        while (_linhaAtual < _relatorio.Itens.Count)
        {
            var item = _relatorio.Itens[_linhaAtual];
            if (y > e.MarginBounds.Bottom - 60)
            {
                e.HasMorePages = true;
                return;
            }

            if (_incluirLocalizacao)
            {
                g.DrawString(item.MaterialNome, fonteNormal, Brushes.Black, margem, y);
                g.DrawString(item.Quantidade.ToString("N2"), fonteNormal, Brushes.Black, margem + 200, y);
                g.DrawString(item.Localizacao ?? "-", fonteNormal, Brushes.Black, margem + 270, y);
                g.DrawString(item.PrecoUnitario.ToString("C2"), fonteNormal, Brushes.Black, margem + 420, y);
                g.DrawString(item.PrecoTotal.ToString("C2"), fonteNormal, Brushes.Black, margem + 520, y);
            }
            else
            {
                g.DrawString(item.MaterialNome, fonteNormal, Brushes.Black, margem, y);
                g.DrawString(item.Quantidade.ToString("N2"), fonteNormal, Brushes.Black, margem + 250, y);
                g.DrawString(item.PrecoUnitario.ToString("C2"), fonteNormal, Brushes.Black, margem + 320, y);
                g.DrawString(item.PrecoTotal.ToString("C2"), fonteNormal, Brushes.Black, margem + 420, y);
            }

            y += 20;
            _linhaAtual++;
        }

        y += 10;
        g.DrawString($"Total Geral: {_relatorio.TotalGeral:C2}", fonteBold, Brushes.Black, margem, y);
        e.HasMorePages = false;
        _linhaAtual = 0;
    }
}
