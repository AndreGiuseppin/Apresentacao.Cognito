using Acesso.Sdk.Result;
using Amazon.CognitoIdentityProvider.Model;

namespace Apresentacao.Cognito.Interfaces.Services
{
    public interface IUserService
    {
        Task<Result<GetUserResponse>> Get(string accessToken);
    }
}
