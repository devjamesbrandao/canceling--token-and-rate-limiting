using Autentication.API.Configuration;
using Autentication.Core.DTO;
using Autentication.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Autentication.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ITokenManager _tokenManager;

        public AccountController(
            IAccountService accountService,
            ITokenManager tokenManager
        )
        {
            _accountService = accountService;
            _tokenManager = tokenManager;
        }

        [HttpGet("account")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [LimitRequests(MaxRequests = 5, TimeWindow = 1)]
        public ActionResult<string> GetUser() => $"Hello, {User!.Identity!.Name}";

        
        [HttpPost("sign-up")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [LimitRequests(MaxRequests = 5, TimeWindow = 60)]
        public async Task<IActionResult> SignUp([FromBody] UserModelView request)
        {
            await _accountService.SignUp(request.Username, request.Password);
            
            return NoContent();
        }


        [HttpPost("sign-in")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(JsonWebToken), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [LimitRequests(MaxRequests = 5, TimeWindow = 1)]
        public async Task<ActionResult<JsonWebToken>> SignIn([FromBody] UserModelView request)
        {
            return await _accountService.SignIn(request.Username, request.Password);
        }


        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [LimitRequests(MaxRequests = 5, TimeWindow = 1)]
        public IActionResult Logout()
        {
            _tokenManager.DeactivateCurrentToken();

            return NoContent();
        }
    }
}