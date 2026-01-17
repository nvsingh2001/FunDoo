using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using BusinessLogicLayer.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.Entities;

namespace BusinessLogicLayer.Services;

public class TokenServices: ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly RsaSecurityKey _privateKey;
    
    public TokenServices(IConfiguration configuration)
    {
        _configuration = configuration;
        
        var rsa = RSA.Create();
        
        rsa.ImportFromPem(File.ReadAllText(_configuration["Jwt:PrivateKeyPath"]));
        
        _privateKey = new  RsaSecurityKey(rsa);
    }

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(
                int.Parse(_configuration["Jwt:ExpiryMinutes"])
            ),
            signingCredentials: new SigningCredentials(
                _privateKey,
                SecurityAlgorithms.RsaSha256
            )
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}