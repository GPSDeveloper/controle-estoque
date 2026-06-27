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
        var conteudo = ThemeHelper.CriarConteudoPrincipal(this, "Login", 420, new Padding(0));

        var tabela = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1,
            Padding = new Padding(16),
            Margin = new Padding(0)
        };
        tabela.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        _txtLogin.Dock = DockStyle.Top;
        _txtSenha.Dock = DockStyle.Top;

        var btnEntrar = ThemeHelper.CriarBotao("Entrar", 160, 40);
        btnEntrar.Click += BtnEntrar_Click;

        var btnSair = ThemeHelper.CriarBotao("Sair", 120, 40);
        btnSair.BackColor = Color.Gray;
        btnSair.Click += (_, _) => Close();

        var barraAcoes = ThemeHelper.CriarBarraAcoesInferior();
        barraAcoes.Controls.Add(btnSair);
        barraAcoes.Controls.Add(btnEntrar);

        tabela.Controls.Add(new Label { Text = "Login", AutoSize = true, Margin = new Padding(0, 0, 0, 4) });
        tabela.Controls.Add(_txtLogin);
        tabela.Controls.Add(new Label { Text = "Senha", AutoSize = true, Margin = new Padding(0, 12, 0, 4) });
        tabela.Controls.Add(_txtSenha);
        tabela.Controls.Add(barraAcoes);

        conteudo.Controls.Add(tabela);

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
