using Apresentacao.Cognito.Configurations;
using Apresentacao.Cognito.Interfaces.Services;
using Apresentacao.Cognito.Models;
using JWT.Algorithms;
using JWT.Builder;
using System.Security.Cryptography;

namespace Apresentacao.Cognito.Services
{
    public class AccessTokenService : IAccessTokenService
    {
        private readonly AccessTokenConfiguration _accessTokenConfiguration;

        public AccessTokenService(AccessTokenConfiguration accessTokenConfiguration)
        {
            _accessTokenConfiguration = accessTokenConfiguration;
        }

        public AccessTokenData Read(string jwt)
        {
            var tokenEncodedSplit = jwt.Split(" ")[1];
            var teste = _accessTokenConfiguration.Keys;

            var a = RSA.Create(teste);


            var jsonDecoded = JwtBuilder
                .Create()
                .WithAlgorithm(new RS256Algorithm(a))
                .Issuer(_accessTokenConfiguration.Issuer)
                .Decode<IDictionary<string, object>>(tokenEncodedSplit);

            return new AccessTokenData(
                jsonDecoded["username"].ToString()
                );
        }

        public string SplitToken(string accessToken)
        {
            return accessToken.Split(" ")[1];
        }
    }
}
