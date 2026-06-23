using ControleEstoque.Services;
using ControleEstoque.UI;

namespace ControleEstoque.Forms;

public class LoginForm : Form
{
    private readonly TextBox _txtLogin = new() { Width = 280 };
    private readonly TextBox _txtSenha = new() { Width = 280, UseSystemPasswordChar = true };
    private readonly AuthService _authService = new();

    public LoginForm()
    {
        ThemeHelper.ConfigurarFormulario(this, "Login - Controle de Estoque", 450, 380);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        Controls.Add(ThemeHelper.CriarCabecalho());

        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(40, 90, 40, 20)
        };

        var lblLogin = new Label { Text = "Login:", AutoSize = true, Location = new Point(0, 20) };
        _txtLogin.Location = new Point(0, 45);

        var lblSenha = new Label { Text = "Senha:", AutoSize = true, Location = new Point(0, 90) };
        _txtSenha.Location = new Point(0, 115);

        var btnEntrar = ThemeHelper.CriarBotao("Entrar", 280, 40);
        btnEntrar.Location = new Point(0, 170);
        btnEntrar.Click += BtnEntrar_Click;

        var btnSair = ThemeHelper.CriarBotao("Sair", 280, 35);
        btnSair.Location = new Point(0, 220);
        btnSair.BackColor = Color.Gray;
        btnSair.Click += (_, _) => Close();

        panel.Controls.AddRange(new Control[] { lblLogin, _txtLogin, lblSenha, _txtSenha, btnEntrar, btnSair });
        Controls.Add(panel);

        AcceptButton = btnEntrar;
        _txtLogin.Focus();
    }

    private void BtnEntrar_Click(object? sender, EventArgs e)
    {
        var usuario = _authService.Autenticar(_txtLogin.Text, _txtSenha.Text);
        if (usuario == null)
        {
            MessageBox.Show("Login ou senha inválidos.", "Acesso negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Hide();
        using var main = new MainForm();
        main.ShowDialog();
        Close();
    }
}
