using static ExchangeApi.DTO.UserDTO;
using static ExchangeApi.Services.JwtTokenService;

namespace ExchangeApi.Services.Interfaces
{
    public interface IJwtTokenService
    {
        string CreateJWTToken(string phonenumber , string name);
        UserInfo GetUserInfoFromToken(string token);

    }
}
