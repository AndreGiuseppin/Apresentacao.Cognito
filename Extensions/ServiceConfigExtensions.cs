using Apresentacao.Cognito.Configurations;
using Apresentacao.Cognito.Interfaces.Services;
using Apresentacao.Cognito.Middlewares;
using Apresentacao.Cognito.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Apresentacao.Cognito.Extensions
{
    public static class ServiceConfigExtensions
    {
        public static void SetupDependencyInjection(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAdminUserService, AdminUserService>();
            services.AddScoped<IHandleResponseService, HandleResponseService>();
            services.AddScoped<IAccessTokenService, AccessTokenService>();
            services.AddTransient<CustomJwtEvents>();

            var awsCognitoUserPool = new AwsCognitoUserPool();
            configuration.Bind("AwsCognitoUserPool", awsCognitoUserPool);
            services.AddSingleton(awsCognitoUserPool);

            var accessTokenConfig = new AccessTokenConfiguration();
            configuration.Bind("AccessTokenConfiguration", accessTokenConfig);
            services.AddSingleton(accessTokenConfig);
        }

        public static void RegisterAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                var accessTokenIssuer = configuration.GetSection("AccessTokenConfiguration").GetSection("issuer").Value;
                var jwksJson = configuration.GetSection("AccessTokenConfiguration").GetSection("keys").Value;
                var jwks = new JsonWebKey(jwksJson);

                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = x.TokenValidationParameters.ValidateIssuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = jwks,
                    ValidIssuer = accessTokenIssuer,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
                x.EventsType = typeof(CustomJwtEvents);
            });
        }
    }
}
