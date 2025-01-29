using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace RifleAssembly.Web.Students
{
    public class TokenProvider(IConfiguration configuration)
    {
        public string Create(Student student)
        {
            string secretKey = configuration["Jwt:Secret"]!;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    [
                        new Claim("groupTitle", student.GroupTitle),
                        new Claim("instituteTitle", student.InstituteTitle),
                        new Claim("firstName", student.FirstName),
                        new Claim("lastName", student.LastName),
                        new Claim("middleName", student.MiddleName),
                    ]),
                Expires = DateTime.UtcNow.AddHours(configuration.GetValue<int>("Jwt:ExpirationsInHours")),
                SigningCredentials = credentials,
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"],
            };

            var handler = new JsonWebTokenHandler();
            string token = handler.CreateToken(tokenDescriptor);

            return token;
        }
    }
}
