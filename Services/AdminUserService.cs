using Acesso.Sdk.Result;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Apresentacao.Cognito.Configurations;
using Apresentacao.Cognito.Interfaces.Services;
using Apresentacao.Cognito.Models;

namespace Apresentacao.Cognito.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IAmazonCognitoIdentityProvider _amazonCognitoIdentityProvider;
        private readonly AwsCognitoUserPool _awsCognitoUserPool;

        public AdminUserService(IAmazonCognitoIdentityProvider amazonCognitoIdentityProvider,
            AwsCognitoUserPool awsCognitoUserPool)
        {
            _amazonCognitoIdentityProvider = amazonCognitoIdentityProvider;
            _awsCognitoUserPool = awsCognitoUserPool;
        }

        public async Task<Result<AdminGetUserResponse>> Get(string userName)
        {
            try
            {
                var adminGetUserRequest = new AdminGetUserRequest
                {
                    Username = userName,
                    UserPoolId = _awsCognitoUserPool.UserPoolId
                };

                var user = await _amazonCognitoIdentityProvider.AdminGetUserAsync(adminGetUserRequest);

                return Result.Ok(user);
            }
            catch (UserNotFoundException)
            {
                return Result.Fail<AdminGetUserResponse>("404", "User not found");
            }
        }

        public async Task<Result<AdminCreateUserResponse>> Post(User userRequest)
        {
            var user = await Get(userRequest.UserName);

            if (user.IsSuccess)
                return Result.Fail<AdminCreateUserResponse>("422", "User already exists");

            var adminCreateUserRequest = new AdminCreateUserRequest
            {
                Username = userRequest.UserName,
                UserPoolId = _awsCognitoUserPool.UserPoolId,
                TemporaryPassword = userRequest.Password,
                DesiredDeliveryMediums = new List<string> { "EMAIL" },
                UserAttributes = new List<AttributeType>
                {
                    new AttributeType { Name = CognitoAttribute.Email.AttributeName, Value = userRequest.Email},
                    new AttributeType { Name = CognitoAttribute.PhoneNumber.AttributeName, Value = userRequest.PhoneNumber},
                    new AttributeType { Name = "custom:DogName", Value = userRequest.DogName}
                }
            };

            var signUp = await _amazonCognitoIdentityProvider.AdminCreateUserAsync(adminCreateUserRequest);

            await _amazonCognitoIdentityProvider.AdminSetUserPasswordAsync(new AdminSetUserPasswordRequest
            {
                Password = userRequest.Password,
                Username = userRequest.UserName,
                UserPoolId = _awsCognitoUserPool.UserPoolId,
                Permanent = true
            });

            await _amazonCognitoIdentityProvider.AdminUpdateUserAttributesAsync(new AdminUpdateUserAttributesRequest
            {
                Username = userRequest.UserName,
                UserPoolId = _awsCognitoUserPool.UserPoolId,
                UserAttributes = new List<AttributeType>
                {
                    new AttributeType {
                        Name = "email_verified",
                        Value = "true"
                    },
                    new AttributeType {
                        Name = "phone_number_verified",
                        Value = "true"
                    }
                }
            });

            return Result.Ok(signUp);
        }

        public async Task<Result> Delete(string userName)
        {
            var user = await Get(userName);

            if (!user.IsSuccess)
                return Result.Fail("404", "User not found");

            var adminDeleteUserRequest = new AdminDeleteUserRequest
            {
                Username = userName,
                UserPoolId = _awsCognitoUserPool.UserPoolId
            };

            await _amazonCognitoIdentityProvider.AdminDeleteUserAsync(adminDeleteUserRequest);

            return Result.Ok();
        }

        public async Task<Result> ForgotPassword(string userName)
        {
            var secretHash = _awsCognitoUserPool.GetSecretHash(userName);

            await _amazonCognitoIdentityProvider.ForgotPasswordAsync(new ForgotPasswordRequest
            {
                ClientId = _awsCognitoUserPool.UserPoolClientId,
                SecretHash = secretHash,
                Username = userName
            });

            return Result.Ok();
        }

        public async Task<Result> ConfirmForgotPassword(ConfirmForgotPassword confirmForgotPassword)
        {
            var secretHash = _awsCognitoUserPool.GetSecretHash(confirmForgotPassword.UserName);

            await _amazonCognitoIdentityProvider.ConfirmForgotPasswordAsync(new ConfirmForgotPasswordRequest
            {
                ClientId = _awsCognitoUserPool.UserPoolClientId,
                SecretHash = secretHash,
                Username = confirmForgotPassword.UserName,
                ConfirmationCode = confirmForgotPassword.Code,
                Password = confirmForgotPassword.Password
            });

            return Result.Ok();
        }
    }
}
