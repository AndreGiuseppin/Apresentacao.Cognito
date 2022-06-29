using Apresentacao.Cognito.Models;

namespace Apresentacao.Cognito.Interfaces.Services
{
    public interface IAccessTokenService
    {
        AccessTokenData Read(string jwt);
        string SplitToken(string accessToken);
    }
}
