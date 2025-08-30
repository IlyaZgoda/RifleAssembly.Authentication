using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RifleAssembly.Authentication.Web.Errors;
using RifleAssembly.Authentication.Web.Extensions;
using RifleAssembly.Authentication.Web.Infrastructure;
using RifleAssembly.Authentication.Web.Infrastructure.Factories;
using RifleAssembly.Authentication.Web.Mappers;
using RifleAssembly.Authentication.Web.Middleware;
using Serilog;
using System.Diagnostics;
using System.Security.Cryptography;

namespace RifleAssembly.Authentication.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, services, configuration) =>
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("ServiceName", "Auth-service")
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName));

            // Add services to the container.
            builder.Services.AddSingleton<TokenProvider>();
            builder.Services.AddSingleton<ErrorToHttpMapper>();
            builder.Services.AddSingleton<ProblemDetailsFactory>();
            builder.Services.AddSingleton<ResultErrorHandler>();
            builder.Services.AddCustomOpenTelemetry(builder.Configuration);
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

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            var app = builder.Build();

            app.UseExceptionHandler();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(swaggerUiOptions =>
                    swaggerUiOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "RiffleAssembly.Authorization API"));
            }

            app.MapGet("/api/health", () => Results.Ok("Healthy!!!!"));
            
            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseSerilogRequestLogging();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();
            app.MapPrometheusScrapingEndpoint();
            app.MapControllers();
            app.Run();
        }
    }
}
