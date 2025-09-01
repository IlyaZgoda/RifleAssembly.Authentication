using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using RifleAssembly.Authentication.Web.Infrastructure.Services;

namespace RifleAssembly.Authentication.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services)
        {
            services.AddSwaggerGen(o =>
            {
                o.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter your JWT token in this field",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT"
                };
                o.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        []
                    }
                };

                o.AddSecurityRequirement(securityRequirement);
            });

            return services;
        }

        public static IServiceCollection AddCustomOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService("Auth-service")
                .AddAttributes(new Dictionary<string, object>
                {
                    ["deployment.environment"] = configuration["Environment"] ?? "Development"
                }))
                .WithMetrics(metrics =>
                {
                    metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddPrometheusExporter();
                })
                .WithTracing(tracing =>
                {
                    tracing
                    .AddAspNetCoreInstrumentation(options
                        => options.Filter = (httpContext)
                        => !httpContext.Request.Path.ToString().Contains("/metrics"))
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(configuration["OpenTelemetry:Otlp:Endpoint"]!);
                        options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                    });
                });

            return services;
        }

        public static IServiceCollection AddLdapServices(this IServiceCollection services)
        {   
            services.AddKeyedScoped<ILdapService, LdapMock>(LdapServices.Mock);
            services.AddKeyedScoped<ILdapService, LdapCrossPlatform>(LdapServices.CrossPlatform);
            services.AddKeyedScoped<ILdapService, LdapWindows>(LdapServices.Windows);

            return services;
        }
    }
}
