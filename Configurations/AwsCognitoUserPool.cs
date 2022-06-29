using System.Text;

namespace Apresentacao.Cognito.Configurations
{
    public class AwsCognitoUserPool
    {
        public string Region { get; set; }
        public string UserPoolClientId { get; set; }
        public string UserPoolClientSecret { get; set; }
        public string UserPoolId { get; set; }

        public string GetSecretHash(string username)
        {
            var dataString = username + UserPoolClientId;

            var data = Encoding.UTF8.GetBytes(dataString);
            var key = Encoding.UTF8.GetBytes(UserPoolClientSecret);

            return Convert.ToBase64String(HmacSHA256(data, key));
        }

        private static byte[] HmacSHA256(byte[] data, byte[] key)
        {
            using (var shaAlgorithm = new System.Security.Cryptography.HMACSHA256(key))
            {
                var result = shaAlgorithm.ComputeHash(data);
                return result;
            }
        }
    }
}
