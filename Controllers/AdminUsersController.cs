using Apresentacao.Cognito.Interfaces.Services;
using Apresentacao.Cognito.Models;
using Microsoft.AspNetCore.Mvc;

namespace Apresentacao.Cognito.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminUsersController
    {
        private readonly IHandleResponseService _handleResponseService;
        private readonly IAdminUserService _adminUserService;

        public AdminUsersController(IHandleResponseService handleResponseService,
            IAdminUserService adminUserService)
        {
            _handleResponseService = handleResponseService;
            _adminUserService = adminUserService;
        }

        [HttpGet("{userName}")]
        public async Task<IActionResult> Get(string userName)
        {
            var result = await _adminUserService.Get(userName);

            return _handleResponseService.HandleResponse(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post(User userRequest)
        {
            var result = await _adminUserService.Post(userRequest);

            return _handleResponseService.HandleResponse(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string userName)
        {
            var result = await _adminUserService.Delete(userName);

            return _handleResponseService.HandleResponse(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string userName)
        {
            var result = await _adminUserService.ForgotPassword(userName);

            return _handleResponseService.HandleResponse(result);
        }

        [HttpPost("confirm-forgot-password")]
        public async Task<IActionResult> ConfirmForgotPassword(ConfirmForgotPassword confirmForgotPassword)
        {
            var result = await _adminUserService.ConfirmForgotPassword(confirmForgotPassword);

            return _handleResponseService.HandleResponse(result);
        }
    }
}
