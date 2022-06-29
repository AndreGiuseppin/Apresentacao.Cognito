using Acesso.Sdk.Result;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Apresentacao.Cognito.Middlewares
{
    public class ExceptionMiddleware
    {
        public RequestDelegate Next { get; }

        public ExceptionMiddleware(RequestDelegate next)
        {
            Next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await Next(httpContext);
            }
            catch (Exception e)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                };

                var result = Result.Fail("500", "Ocorreu um erro interno.");
                var resultAsJson = JsonSerializer.Serialize(result.Error, options);
                var resultAsBytes = Encoding.UTF8.GetBytes(resultAsJson);

                using var stream = new MemoryStream();

                stream.Write(resultAsBytes);

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                httpContext.Response.Headers.Add(
                    "content-type",
                    new Microsoft.Extensions.Primitives.StringValues("application/json; charset=utf-8"));

                await httpContext.Response.Body.WriteAsync(resultAsBytes);
            }
        }
    }
}
