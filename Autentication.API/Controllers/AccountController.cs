using Autentication.API.Configuration;
using Autentication.Core.DTO;
using Autentication.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Autentication.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/api/[controller]")]
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

        /// <summary>
        /// Checks if the user is logged in
        /// </summary>
        /// <remarks>
        /// Requisition example:
        /// 
        ///     [GET] v1/api/Account/account
        /// </remarks>
        /// <response code="200">
        /// User is logged in.
        /// </response>   
        /// <response code="401">
        /// User is not logged in.
        /// </response>   
        [HttpGet("account")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [LimitRequests(MaxRequests = 5, TimeWindowInSeconds = 60)]
        public ActionResult<string> GetUser() => $"Hello, {User!.Identity!.Name}";

        /// <summary>
        /// Sign up in the application
        /// </summary>
        /// <remarks>
        /// Requisition example:
        /// 
        ///     [POST] v1/api/Account/sign-up
        ///     {        
        ///       "Username": "Harry",
        ///       "Password": "Potter"
        ///     }
        /// </remarks>
        /// <param name="request"></param> 
        /// <response code="400">
        /// Possible errors: "Username can not be empty."; "Password can not be empty."; "Username 'Dumbledore' is already in use.";
        /// </response>    
        /// <response code="204">
        /// Success sign up
        /// </response>   
        [HttpPost("sign-up")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [LimitRequests(MaxRequests = 5, TimeWindowInSeconds = 60)]
        public async Task<IActionResult> SignUp([FromBody] UserModelView request)
        {
            await _accountService.SignUp(request.Username, request.Password);
            
            return NoContent();
        }

        /// <summary>
        /// Sign in in the application
        /// </summary>
        /// <remarks>
        /// Requisition example:
        /// 
        ///     [POST] v1/api/Account/sign-in
        ///     {        
        ///       "Username": "Harry",
        ///       "Password": "Potter"
        ///     }
        /// </remarks>
        /// <param name="request"></param> 
        /// <response code="400">
        /// Possible errors: "Invalid username or password";
        /// </response>    
        /// <response code="200">
        /// Success sign in 
        /// </response>   
        [HttpPost("sign-in")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(JsonWebToken), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [LimitRequests(MaxRequests = 5, TimeWindowInSeconds = 60)]
        public async Task<ActionResult<JsonWebToken>> SignIn([FromBody] UserModelView request)
        {
            return await _accountService.SignIn(request.Username, request.Password);
        }

        /// <summary>
        /// Application logout
        /// </summary>
        /// <remarks>
        /// Requisition example:
        /// 
        ///     [DELETE] v1/api/Account/logout
        /// </remarks>
        /// <response code="401">
        /// Unauthorized user
        /// </response>    
        /// <response code="204">
        /// Success logout
        /// </response>   
        [HttpDelete("logout")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [LimitRequests(MaxRequests = 5, TimeWindowInSeconds = 60)]
        public IActionResult Logout()
        {
            _tokenManager.DeactivateCurrentToken();

            return NoContent();
        }
    }
}