namespace ControleEstoque.Forms;

public class InputDialogForm : Form
{
    private readonly TextBox _txtValor = new() { Width = 300 };

    public string Valor => _txtValor.Text;

    public InputDialogForm(string titulo, string label, string valorInicial = "")
    {
        Text = titulo;
        Size = new Size(430, 190);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        var painel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20)
        };

        var lbl = new Label { Text = label, AutoSize = true, Dock = DockStyle.Top, Margin = new Padding(0, 0, 0, 4) };
        _txtValor.Text = valorInicial;
        _txtValor.Dock = DockStyle.Top;

        var btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 80 };
        var btnCancel = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Width = 90 };

        var barraAcoes = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Margin = new Padding(0, 12, 0, 0)
        };
        barraAcoes.Controls.Add(btnCancel);
        barraAcoes.Controls.Add(btnOk);

        painel.Controls.Add(barraAcoes);
        painel.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 12 });
        painel.Controls.Add(_txtValor);
        painel.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 4 });
        painel.Controls.Add(lbl);
        Controls.Add(painel);
        AcceptButton = btnOk;
        CancelButton = btnCancel;
    }

    public static string? Mostrar(string titulo, string label, string valorInicial = "")
    {
        using var dialog = new InputDialogForm(titulo, label, valorInicial);
        return dialog.ShowDialog() == DialogResult.OK ? dialog.Valor : null;
    }
}
