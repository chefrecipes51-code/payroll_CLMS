using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Payroll.WebApp.Helpers
{
    public static class JwtTokenGeneratorHelper
    {
        public static string GetToken(string email, List<string> roles, IConfiguration _config)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create the base claims (email, unique identifier)
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add role claims dynamically based on the passed roles list
            claims.AddRange(roles.Select(role => new Claim("TypeOfRole", role)));

            // Create the token
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials
            );

            // Return the token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        //public static string GetToken(string email, string role, IConfiguration config)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(config["Jwt:SecretKey"]);
        //    var issuer = config["Jwt:Issuer"];
        //    var audience = config["Jwt:Audience"];
        //    var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Email, email),
        //        new Claim("TypeOfRole", role)
        //    };

        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Issuer = issuer,
        //        Audience = audience,
        //        Subject = new ClaimsIdentity(claims.ToArray()),
        //        Expires = DateTime.UtcNow.AddDays(7),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    };

        //    var token = tokenHandler.CreateToken(tokenDescriptor);

        //    return tokenHandler.WriteToken(token);
        //}
    }
}
