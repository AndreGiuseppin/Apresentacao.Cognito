namespace Apresentacao.Cognito.Models
{
    public record ConfirmForgotPassword(string UserName, string Password, string Code);
}
