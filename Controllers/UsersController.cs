using Apresentacao.Cognito.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Apresentacao.Cognito.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController
    {
        private readonly IUserService _userService;
        private readonly IHandleResponseService _handleResponseService;

        public UsersController(IUserService userService,
            IHandleResponseService handleResponseService)
        {
            _userService = userService;
            _handleResponseService = handleResponseService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromHeader] string authorization)
        {
            var result = await _userService.Get(authorization);

            return _handleResponseService.HandleResponse(result);
        }
    }
}