using Amazon.CognitoIdentityProvider.Model;

namespace Apresentacao.Cognito.Models
{
    public record SignInResponse
    {
        public SecretResponse SecretResponse { get; set; }
        public AuthenticationResponse AuthenticationResponse { get; set; }
    }

    public class SecretResponse
    {
        public string AssociationSession { get; set; }
        public string Payload { get; set; }
    }


    public class AuthenticationResponse
    {
        public AuthenticationResultType AuthenticationResult { get; set; }
    }
}
