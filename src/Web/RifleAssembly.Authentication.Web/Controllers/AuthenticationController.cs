using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RifleAssembly.Authorization.Web.Services;
using RifleAssembly.Authorization.Web.Students;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
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
            var principal = HttpContext.User;

            // Извлечение необходимых claim
            var instituteTitle = principal.FindFirstValue("instituteTitle");
            var groupTitle = principal.FindFirstValue("groupTitle");
            var firstName = principal.FindFirstValue(ClaimTypes.Name);
            var lastName = principal.FindFirstValue(ClaimTypes.Surname);
            var middleName = principal.FindFirstValue("middleName");
            var login = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            var student = new Student(instituteTitle, groupTitle, firstName, lastName, middleName, login);

            return Ok(student);
        }
    }
}
