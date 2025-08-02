using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RifleAssembly.Authentication.Web.Infrastructure.Services;
using RifleAssembly.Authentication.Web.Students;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using LoginRequest = RifleAssembly.Authentication.Web.Students.LoginRequest;

namespace RifleAssembly.Authentication.Web.Controllers
{
    [Route("api")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILdapService _ldap;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController([FromKeyedServices(LdapServices.CrossPlatform)] ILdapService ldap, ILogger<AuthenticationController> logger)
        {
            _ldap = ldap;
            _logger = logger;
        }
            

        [HttpPost("login")]
        public IActionResult Login([FromBody, Required] LoginRequest loginRequest)
        {
            _logger.LogInformation("Authentication requested for user: {Login}", loginRequest.Login);

            var token = _ldap.Authenticate(loginRequest.Login, loginRequest.Password);

            return token is not null ? Ok(token) : BadRequest("Incorrect login or password");
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
