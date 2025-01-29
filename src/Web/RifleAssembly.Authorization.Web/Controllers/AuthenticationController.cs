using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RifleAssembly.Web.Students;
using System.ComponentModel.DataAnnotations;
using LoginRequest = RifleAssembly.Web.Students.LoginRequest;

namespace RifleAssembly.Web.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private readonly TokenProvider _tokenProvider;

        public AuthenticationController(TokenProvider tokenProvider) =>
            _tokenProvider = tokenProvider;

        [HttpPost]
        public IActionResult Login([FromBody][Required] LoginRequest loginRequest)
        {
            var student = new Student("ИМИТ", "4.205-1", "Илья", "Згода", "Константинович");
            var token = _tokenProvider.Create(student);

            return Ok(token);
        }
    }
}
