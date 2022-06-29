using Apresentacao.Cognito.Interfaces.Services;
using Apresentacao.Cognito.Models;
using Microsoft.AspNetCore.Mvc;

namespace Apresentacao.Cognito.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController
    {
        private readonly IAuthService _authService;
        private readonly IHandleResponseService _handleResponseService;

        public AuthController(IAuthService authService,
            IHandleResponseService handleResponseService)
        {
            _authService = authService;
            _handleResponseService = handleResponseService;
        }

        [HttpPost("signIn")]
        public async Task<IActionResult> SignIn(SignUpRequest signUpRequest)
        {
            var result = await _authService.SignIn(signUpRequest);

            return _handleResponseService.HandleResponse(result);
        }

        [HttpPost]
        [Route("VerifyMfaToken")]
        public async Task VerifyMfaToken(VerifyTokenRequest verifyTokenRequest)
        {
            await _authService.VerifyMfaToken(verifyTokenRequest);
        }
    }
}
