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
        /// 
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     [GET] v1/api/Account/account
        /// </remarks>
        /// <response code="200">
        /// Funcionário está logado.
        /// </response>   
        /// <response code="401">
        /// Funcionário não está logado.
        /// </response>   
        /// <response code="500">
        /// Erro inesperado na aplicação.
        /// </response>   
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

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     [POST] v1/api/
        ///     {        
        ///       "userName": "test"
        ///     }
        /// </remarks>
        /// <param name="credenciais"></param> 
        /// <response code="400">
        /// Possíveis erros: "Usuário ou senha inválidos"; "Funcionário não possui codLoja cadastrado."; 
        /// </response>    
        /// <response code="500">
        /// Erro inesperado na aplicação 
        /// </response>   
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