using ExchangeApi.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static ExchangeApi.DTO.UserDTO;

namespace ExchangeApi.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string CreateJWTToken(string phonenumber,string name)
        {

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, phonenumber),
            new Claim(ClaimTypes.Name, name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public UserInfo GetUserInfoFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userInfo = new UserInfo();
            
            userInfo.name = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            userInfo.phoneNumber = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            return userInfo;
        }

        public class UserInfo
        {
            public string name;
            public string phoneNumber;
        }
    }

}
