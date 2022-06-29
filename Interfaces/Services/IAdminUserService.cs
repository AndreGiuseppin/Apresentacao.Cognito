using Acesso.Sdk.Result;
using Amazon.CognitoIdentityProvider.Model;
using Apresentacao.Cognito.Models;

namespace Apresentacao.Cognito.Interfaces.Services
{
    public interface IAdminUserService
    {
        Task<Result<AdminGetUserResponse>> Get(string userName);
        Task<Result<AdminCreateUserResponse>> Post(User userRequest);
        Task<Result> Delete(string userName);
        Task<Result> ForgotPassword(string userName);
        Task<Result> ConfirmForgotPassword(ConfirmForgotPassword confirmForgotPassword);
    }
}
