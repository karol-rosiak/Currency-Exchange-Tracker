using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CurrencyTracker.API.Swagger
{
    /// <summary>
    /// Marks operations with security requirements when they (or their controller) have [Authorize].
    /// </summary>
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAuthorize =
                context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() == true
                || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

            var allowAnonymous =
                context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any()
                || context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any() == true;

            if (!hasAuthorize || allowAnonymous)
            {
                return;
            }

            if (!operation.Responses.ContainsKey("401"))
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            }

            if (!operation.Responses.ContainsKey("403"))
            {
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
            }

            var securityScheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            };

            operation.Security ??= new List<OpenApiSecurityRequirement>();

            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [securityScheme] = new string[] { }
            });
        }
    }
}