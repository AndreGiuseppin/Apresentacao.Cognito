using Acesso.Sdk.Result;
using Apresentacao.Cognito.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Apresentacao.Cognito.Middlewares
{
    public class CustomJwtEvents : JwtBearerEvents
    {
        private readonly IAccessTokenService _jwtService;

        public CustomJwtEvents(
            IAccessTokenService jwtService)
        {
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        }

        public override async Task Challenge(JwtBearerChallengeContext context)
        {
            context.HandleResponse();

            var authorizationHeader = context
                .Request
                .Headers["Authorization"]
                .FirstOrDefault();

            try
            {
                var tokenPublicData = authorizationHeader?.Split('.')[1];


            }
            catch
            {

            }

            await HandleCustomResponse(context);

            if (context.AuthenticateFailure?.GetType() != typeof(SecurityTokenExpiredException))
                return;

            _jwtService.Read(authorizationHeader);
        }

        private static async Task HandleCustomResponse(JwtBearerChallengeContext context)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            var result = Result.Fail("401", "Código de autenticação inválido");
            var resultAsJson = JsonSerializer.Serialize(result.Error, options);
            var resultAsBytes = Encoding.UTF8.GetBytes(resultAsJson);
            var stream = new MemoryStream();
            stream.Write(resultAsBytes);

            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            context.Response.Headers.Add(
                "content-type",
                new Microsoft.Extensions.Primitives.StringValues("application/json; charset=utf-8"));

            await context.Response.Body.WriteAsync(resultAsBytes);
        }
    }
}
