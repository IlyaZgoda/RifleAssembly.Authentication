using Microsoft.IdentityModel.Tokens;
using RifleAssembly.Authentication.Web.Students;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace RifleAssembly.Authentication.Web.Infrastructure
{
    public class TokenProvider(IConfiguration configuration, IWebHostEnvironment environment)
    {
        public string Create(Student student)
        {
            string privateKeyXml = environment.IsDevelopment() 
                ? File.ReadAllText(configuration["Jwt:Secret"]!) 
                : Environment.GetEnvironmentVariable("JWT_PRIVATE_KEY")!;

            var rsa = RSA.Create();
            rsa.FromXmlString(privateKeyXml);

            var privateKey = new RsaSecurityKey(rsa);

            var credentials = new SigningCredentials(privateKey, SecurityAlgorithms.RsaSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    [
                        new Claim("groupTitle", student.GroupTitle),
                        new Claim("instituteTitle", student.InstituteTitle),
                        new Claim(ClaimTypes.Name, student.FirstName),
                        new Claim(ClaimTypes.Surname, student.LastName),
                        new Claim("middleName", student.MiddleName),
                        new Claim(ClaimTypes.NameIdentifier, student.Login)
                    ]),
                Expires = DateTime.UtcNow.AddHours(configuration.GetValue<int>("Jwt:ExpirationInHours")),
                SigningCredentials = credentials,
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"],
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token);
        }
    }
}
