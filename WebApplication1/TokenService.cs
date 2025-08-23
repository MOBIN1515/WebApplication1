using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace WebApplication1;

public class TokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }
    public string CreateUser(User user)
    {
        var claims = new[]
        {
             new Claim(ClaimTypes.Name, user.UserName),
             new Claim("UserId", user.Id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config [ "jwt;key"]));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(

            issuer: _config["jwt : issure"],
            audience : _config["jwt:audince"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: cred

            );
        return new JwtSecurityTokenHandler().WriteToken(token);





















    }    
       
}
