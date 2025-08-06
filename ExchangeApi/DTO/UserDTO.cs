using ExchangeApi.Models.Entities;

namespace ExchangeApi.DTO
{
    public class UserDTO
    {
        public class RegisterDto
        {
            public required string Name { get; set; }
            public required string PhoneNumber { get; set; }
            public required string Password { get; set; }
        }

        public class LoginDto
        {
            public required UsersInfo User { get; set; }

            public required string Token { get; set; }
        }

    }
}
