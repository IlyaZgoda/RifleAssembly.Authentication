using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RifleAssembly.Authentication.Web.Extensions;
using RifleAssembly.Authentication.Web.Infrastructure;
using System.Security.Cryptography;

namespace RifleAssembly.Authentication.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.ListenAnyIP(8080);
            });
            // Add services to the container.
            builder.Services.AddSingleton<TokenProvider>();
            builder.Services.AddLdapServices();
            builder.Services.AddRazorPages();
            builder.Services.AddControllers();

            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    string publicKeyXml = builder.Configuration["Jwt:PublicKeyPath"]!;
                    var rsa = RSA.Create();
                    var publicKeyString = File.ReadAllText(publicKeyXml);
                    Console.WriteLine(publicKeyString);
                    rsa.FromXmlString(publicKeyString);

                    o.RequireHttpsMetadata = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new RsaSecurityKey(rsa),
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddSwaggerGenWithAuth();

            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(swaggerUiOptions =>
                    swaggerUiOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "RiffleAssembly.Authorization API"));
            }

            app.MapGet("/api/health", () => Results.Ok("Healthy"));
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.MapControllers();
            app.Run();
        }
    }
}
