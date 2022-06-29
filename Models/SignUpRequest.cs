namespace Apresentacao.Cognito.Models
{
    public record SignUpRequest(string UserName, string Password, string? MfaToken);
}
