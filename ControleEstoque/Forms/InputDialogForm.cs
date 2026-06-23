namespace ControleEstoque.Forms;

public class InputDialogForm : Form
{
    private readonly TextBox _txtValor = new() { Width = 300 };

    public string Valor => _txtValor.Text;

    public InputDialogForm(string titulo, string label, string valorInicial = "")
    {
        Text = titulo;
        Size = new Size(400, 180);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        var lbl = new Label { Text = label, AutoSize = true, Location = new Point(20, 20) };
        _txtValor.Text = valorInicial;
        _txtValor.Location = new Point(20, 45);

        var btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new Point(200, 85), Width = 60 };
        var btnCancel = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Location = new Point(270, 85), Width = 80 };

        Controls.AddRange(new Control[] { lbl, _txtValor, btnOk, btnCancel });
        AcceptButton = btnOk;
        CancelButton = btnCancel;
    }

    public static string? Mostrar(string titulo, string label, string valorInicial = "")
    {
        using var dialog = new InputDialogForm(titulo, label, valorInicial);
        return dialog.ShowDialog() == DialogResult.OK ? dialog.Valor : null;
    }
}
