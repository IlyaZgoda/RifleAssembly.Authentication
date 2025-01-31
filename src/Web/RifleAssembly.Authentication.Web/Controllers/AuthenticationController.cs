using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RifleAssembly.Authorization.Web.Services;
using System.ComponentModel.DataAnnotations;
using LoginRequest = RifleAssembly.Authorization.Web.Students.LoginRequest;

namespace RifleAssembly.Authorization.Web.Controllers
{
    [Route("api")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class AuthenticationController : ControllerBase
    {
        private readonly Ldap _ldap;

        public AuthenticationController(Ldap ldap) =>
            _ldap = ldap;

        [HttpPost("login")]
        public IActionResult Login([FromBody][Required] LoginRequest loginRequest)
        {
            var token = _ldap.Authenticate(loginRequest.Login, loginRequest.Password);

            return token is not null? Ok(token) : BadRequest("Incorrect login or password");
        }
    }

    [Route("api")]
    [Authorize]
    public class StudentController : ControllerBase
    {
        [HttpGet("user")]
        public IActionResult Process()
        {
            var user = HttpContext.User;

            return Ok(user);
        }
    }
}
