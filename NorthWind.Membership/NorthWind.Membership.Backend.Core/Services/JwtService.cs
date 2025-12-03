using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NorthWind.Membership.Backend.Core.Dtos;
using NorthWind.Membership.Backend.Core.Options;
using System.Security.Claims;
using System.Text;

namespace NorthWind.Membership.Backend.Core.Services
{
    internal class JwtService(IOptions<JwtOptions> options)
    {
        SigningCredentials GetSigningCredentials()
        {
            var Key = Encoding.UTF8.GetBytes(options.Value.SecurityKey);
            var Secret = new SymmetricSecurityKey(Key);
            return new SigningCredentials(Secret, SecurityAlgorithms.HmacSha256);
        }

        List<Claim> GetClaims(UserDto userDto)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userDto.Email),
                new Claim("FullName", $"{userDto.FirstName} {userDto.LastName}"),
                //new Claim("Cedula", userDto.Cedula) OPCIONAL SI LO NECESITO EN EL FRONT
            };

            // Agregar roles como claims
            foreach (var role in userDto.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        SecurityTokenDescriptor GetTokenDescriptor(
            SigningCredentials signingCredentials,
            List<Claim> claims) => new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = options.Value.ValidIssuer,
                Audience = options.Value.ValidAudience,
                Expires = DateTime.UtcNow.AddMinutes(options.Value.ExpireInMinutes),
                SigningCredentials = signingCredentials
            };

        public string GetToken(UserDto userData)
        {
            var SigningCredentials = GetSigningCredentials();
            var Claims = GetClaims(userData);
            var TokenDescriptor = GetTokenDescriptor(SigningCredentials, Claims);
            var TokenHandler = new JsonWebTokenHandler();
            return TokenHandler.CreateToken(TokenDescriptor);
        }
    }
}
