using Acesso.Sdk.Result;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Apresentacao.Cognito.Configurations;
using Apresentacao.Cognito.Interfaces.Services;

namespace Apresentacao.Cognito.Services
{
    public class UserService : IUserService
    {
        private readonly IAmazonCognitoIdentityProvider _amazonCognitoIdentityProvider;
        private readonly IAccessTokenService _accessTokenService;
        private readonly AwsCognitoUserPool _awsCognitoUserPool;

        public UserService(IAmazonCognitoIdentityProvider amazonCognitoIdentityProvider,
            AwsCognitoUserPool awsCognitoUserPool,
            IAccessTokenService accessTokenService)
        {
            _amazonCognitoIdentityProvider = amazonCognitoIdentityProvider;
            _awsCognitoUserPool = awsCognitoUserPool;
            _accessTokenService = accessTokenService;
        }

        public async Task<Result<GetUserResponse>> Get(string accessToken)
        {
            var tokenSplited = _accessTokenService.SplitToken(accessToken);

            var user = await _amazonCognitoIdentityProvider.GetUserAsync(new GetUserRequest
            {
                AccessToken = tokenSplited
            });

            if (user is null)
                return Result.Fail<GetUserResponse>("404", "User not found");

            return Result.Ok(user);
        }
    }
}
