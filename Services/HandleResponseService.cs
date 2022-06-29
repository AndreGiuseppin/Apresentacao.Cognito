using Acesso.Sdk.Result;
using Apresentacao.Cognito.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Apresentacao.Cognito.Services
{
    public class HandleResponseService : ControllerBase, IHandleResponseService
    {
        public IActionResult HandleResponse(Result result)
        {
            if (result.IsSuccess)
                return Ok();

            var parseCodeResult = int.TryParse(result.Error.Code, out var parsedCode);

            if (parseCodeResult is false)
                parsedCode = 500;

            return StatusCode(parsedCode, result.Error);
        }

        public IActionResult HandleResponse<T>(Result<T> result)
        {
            if (result.IsSuccess)
                return Ok(result.Value);

            var parseCodeResult = int.TryParse(result.Error.Code, out var parsedCode);

            if (parseCodeResult is false)
                parsedCode = 500;

            return StatusCode(parsedCode, result.Error);
        }
    }
}
