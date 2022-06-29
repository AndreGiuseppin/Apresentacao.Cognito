using Acesso.Sdk.Result;
using Microsoft.AspNetCore.Mvc;

namespace Apresentacao.Cognito.Interfaces.Services
{
    public interface IHandleResponseService
    {
        public IActionResult HandleResponse(Result result);
        public IActionResult HandleResponse<T>(Result<T> result);
    }
}
