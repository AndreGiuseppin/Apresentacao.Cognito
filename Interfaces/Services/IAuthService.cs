using Acesso.Sdk.Result;
using Apresentacao.Cognito.Models;

namespace Apresentacao.Cognito.Interfaces.Services
{
    public interface IAuthService
    {
        Task<Result<SignInResponse>> SignIn(SignUpRequest signUpRequest);
        Task VerifyMfaToken(VerifyTokenRequest verifyTokenRequest);
    }
}
