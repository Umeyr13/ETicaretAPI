using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ETicaretAPI.Infrastructure.Services.Token
{
    public class TokenHandler : ITokenHandler
    {
        readonly IConfiguration _configuration;

        public TokenHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Application.DTOs.Token CreateAccessToken(int second, AppUser user)
        {
            Application.DTOs.Token token = new (); //bizim token nesne miz
            //Security Key in simetriğini alıyoruz
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_configuration["Token:SecurityKey"]));
            //Şifrelşenmiş kimliği oluşturuyoruz.
            SigningCredentials signingCredentials = new(securityKey,SecurityAlgorithms.HmacSha256);
            //oluşturulacak token ayarları

            token.Expiration= DateTime.UtcNow.AddSeconds(second);
            JwtSecurityToken securityToken = new(
                audience: _configuration["Token:Audience"],
                issuer: _configuration["Token:Issuer"],
                expires:token.Expiration,
                notBefore: DateTime.UtcNow,//ne zaman devreye girsin, üretilir üretilmez
                signingCredentials: signingCredentials,
                claims: new List<Claim> { new(ClaimTypes.Name,user.UserName)}//loglama mekanizması için ekledik
                );

            //Token oluşturucu sınıfından bir örnek alalım
            JwtSecurityTokenHandler tokenHandler = new();
            token.AccsessToken = tokenHandler.WriteToken(securityToken); //string token dönüyor

            //accesstoken üretildiğinde bir tane de refresh token üretilsin diye tetikliyoruz =>
            token.RefreshToken = CreateRefreshToken();
            
            return token;          
        }

        public string CreateRefreshToken()
        {
            byte[] number = new byte[32];
            using RandomNumberGenerator random = RandomNumberGenerator.Create();//using sayesinde bu metot bitene kadar sayı kullanılır bu metottan çıkınca nesne Dispose edilir. IDisposable olması lazım
            random.GetBytes(number);
            return Convert.ToBase64String(number);
        }
    }
}
