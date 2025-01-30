using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace RifleAssembly.Authorization.Web.Students
{
    public class TokenProvider(IConfiguration configuration)
    {
        public string Create(Student student)
        {
            string privateKeyXml = configuration["Jwt:Secret"]!;
            var rsa = RSA.Create();
            var privateKeyString = File.ReadAllText(privateKeyXml);
            rsa.FromXmlString(privateKeyString);

            var privateKey = new RsaSecurityKey(rsa);

            var credentials = new SigningCredentials(privateKey, SecurityAlgorithms.RsaSha256);

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
