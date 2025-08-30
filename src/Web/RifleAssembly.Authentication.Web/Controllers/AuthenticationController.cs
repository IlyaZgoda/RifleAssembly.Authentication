using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RifleAssembly.Authentication.Web.Extensions;
using RifleAssembly.Authentication.Web.Infrastructure.Factories;
using RifleAssembly.Authentication.Web.Infrastructure.Services;
using RifleAssembly.Authentication.Web.Mappers;
using RifleAssembly.Authentication.Web.Students;
using RifleAssembly.WebService.SharedKernel.Result;
using RifleAssembly.WebService.SharedKernel.Result.Methods.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using LoginRequest = RifleAssembly.Authentication.Web.Students.LoginRequest;

namespace RifleAssembly.Authentication.Web.Controllers
{
    [Route("api")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(ProblemDetails))]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILdapService _ldap;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly ResultErrorHandler _resultErrorHandler;

        public AuthenticationController([FromKeyedServices(LdapServices.Mock)] ILdapService ldap, 
            ILogger<AuthenticationController> logger,
            ResultErrorHandler resultErrorHandler)
        {
            _ldap = ldap;
            _logger = logger;
            _resultErrorHandler = resultErrorHandler;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody][Required] LoginRequest loginRequest) =>
            await _ldap.AuthenticateAsync(loginRequest.Login, loginRequest.Password)
                .Tap(token => _logger.LogInformation("Authentication successful for user: {Login}", loginRequest.Login),
                    error => _logger.LogWarning("Authentication for user: {Login} failed with error: {Error}", loginRequest.Login, error.Description))
                .Match(
                    token => Ok(token),
                    error => _resultErrorHandler.Handle(error, HttpContext)
                );
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
