using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace EAD.Services
{
    public class JwtService
    {
        private readonly string _secretkey;

        public JwtService(string secretkey)
        {
            _secretkey = secretkey;
        }

        public string GenerateToken(string email, string role)
        {
            var claims = new List<Claim>{
                new Claim(ClaimTypes.Email,email),
                new Claim(ClaimTypes.Role,role),
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_secretkey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
