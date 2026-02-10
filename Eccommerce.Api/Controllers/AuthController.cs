using Eccormmerce.Application.Commands.Auth.Login;
using Eccormmerce.Application.Commands.Auth.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eccommerce.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommand command)
        {
            var userId = await _mediator.Send(command);

            return Ok(new
            {
                success = true,
                data = new { userId },
                message = "User registered successfully"
            });
        }
        
        [AllowAnonymous, HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var token = await _mediator.Send(command);

            return Ok(new
            {
                success = true,
                token
            });
        }
    }
}
