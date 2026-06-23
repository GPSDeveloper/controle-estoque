namespace ControleEstoque.UI;

public static class OperationFeedbackHelper
{
    public static bool ExecutarComRetryConcorrencia(
        Func<(bool Sucesso, string Mensagem)> operacao,
        Action? onSuccess = null,
        string tituloSucesso = "Sucesso",
        string tituloAtencao = "Atenção")
    {
        while (true)
        {
            var (sucesso, mensagem) = operacao();

            if (sucesso)
            {
                MessageBox.Show(mensagem, tituloSucesso, MessageBoxButtons.OK, MessageBoxIcon.Information);
                onSuccess?.Invoke();
                return true;
            }

            if (EhConflitoConcorrencia(mensagem))
            {
                var tentarNovamente = MessageBox.Show(
                    $"{mensagem}\n\nDeseja tentar novamente agora?",
                    "Conflito de concorrência",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (tentarNovamente == DialogResult.Yes)
                    continue;
            }

            MessageBox.Show(mensagem, tituloAtencao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
    }

    private static bool EhConflitoConcorrencia(string mensagem) =>
        mensagem.Contains("Conflito de concorrência", StringComparison.OrdinalIgnoreCase);
}
