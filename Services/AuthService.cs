using Acesso.Sdk.Result;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Apresentacao.Cognito.Configurations;
using Apresentacao.Cognito.Interfaces.Services;
using Apresentacao.Cognito.Models;
using QRCoder;
using SignUpRequest = Apresentacao.Cognito.Models.SignUpRequest;

namespace Apresentacao.Cognito.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAmazonCognitoIdentityProvider _amazonCognitoIdentityProvider;
        private readonly AwsCognitoUserPool _awsCognitoUserPool;

        public AuthService(IAmazonCognitoIdentityProvider amazonCognitoIdentityProvider,
            AwsCognitoUserPool awsCognitoUserPool)
        {
            _amazonCognitoIdentityProvider = amazonCognitoIdentityProvider;
            _awsCognitoUserPool = awsCognitoUserPool;
        }

        public async Task<Result<SignInResponse>> SignIn(SignUpRequest signUpRequest)
        {
            var secretHash = _awsCognitoUserPool.GetSecretHash(signUpRequest.UserName);
            var signIn = await _amazonCognitoIdentityProvider.InitiateAuthAsync(new InitiateAuthRequest
            {
                ClientId = _awsCognitoUserPool.UserPoolClientId,
                AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                AuthParameters = new Dictionary<string, string>
                {
                    { "USERNAME", signUpRequest.UserName },
                    { "PASSWORD", signUpRequest.Password },
                    { "SECRET_HASH", secretHash }
                }
            });

            if (signIn.ChallengeName == ChallengeNameType.MFA_SETUP)
            {
                var qrCode = await Generate_QrCode(signUpRequest.UserName, signIn.Session);
                return Result.Ok(new SignInResponse { SecretResponse = qrCode.Value });
            }

            var authChalleng = await _amazonCognitoIdentityProvider.RespondToAuthChallengeAsync(new RespondToAuthChallengeRequest
            {
                ChallengeName = signIn.ChallengeName,
                ChallengeResponses = new Dictionary<string, string>
                {
                    { "SOFTWARE_TOKEN_MFA_CODE", signUpRequest.MfaToken },
                    { "USERNAME", signUpRequest.UserName },
                    { "SECRET_HASH", secretHash }
                },
                Session = signIn.Session,
                ClientId = _awsCognitoUserPool.UserPoolClientId
            });

            return Result.Ok(new SignInResponse { AuthenticationResponse = new AuthenticationResponse { AuthenticationResult = authChalleng.AuthenticationResult } });
        }

        public async Task VerifyMfaToken(VerifyTokenRequest verifyTokenRequest)
        {
            try
            {
                var verificationResult = await _amazonCognitoIdentityProvider.VerifySoftwareTokenAsync(new VerifySoftwareTokenRequest
                {
                    Session = verifyTokenRequest.Session,
                    UserCode = verifyTokenRequest.Token
                });
            }
            catch (Exception)
            {
            }
        }

        private async Task<Result<SecretResponse>> Generate_QrCode(string userName, string session)
        {
            var secretResponse = await _amazonCognitoIdentityProvider.AssociateSoftwareTokenAsync(new AssociateSoftwareTokenRequest()
            {
                Session = session
            });

            PayloadGenerator.OneTimePassword generator = new PayloadGenerator.OneTimePassword()
            {
                Secret = secretResponse.SecretCode,
                Issuer = "Apresentação",
                Label = userName,
            };

            string payload = generator.ToString();
            return Result.Ok(new SecretResponse { AssociationSession = secretResponse.Session, Payload = payload });
        }
    }
}
