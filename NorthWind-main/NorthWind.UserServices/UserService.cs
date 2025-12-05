using Microsoft.AspNetCore.Http;
using NorthWind.Entities.Interfaces;
using System.Security.Claims;

namespace NorthWind.UserServices
{
    public class UserService(IHttpContextAccessor context) : IUserService
    {
        public string UserId =>
            context.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public bool IsAuthenticated =>
            context.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public string UserName =>
            context.HttpContext?.User?.Identity?.Name;

        // Puedes mapear el nombre completo si lo tienes en los claims, o usar el UserName por defecto
        public string FullName =>
            context.HttpContext?.User?.FindFirst(ClaimTypes.GivenName)?.Value ?? UserName;
    }
}