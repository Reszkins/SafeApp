using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SafeApp.Utils
{
    public interface ITokenGenerator
    {
        string GenerateToken(string userName);
    }
    public class TokenGenerator : ITokenGenerator
    {
        public string GenerateToken(string userName)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("$ecr3tKeeyYf000rMy@pp"));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512Signature);

            var tokenOptions = new JwtSecurityToken(
                issuer: "http://localhost:8383",
                audience: "http://localhost:8383",
                claims: new List<Claim> { new Claim("userName", userName)},
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
    }
}
