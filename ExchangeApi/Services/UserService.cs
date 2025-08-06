using ExchangeApi.Controllers;
using ExchangeApi.Data;
using ExchangeApi.Models.Entities;
using ExchangeApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using static ExchangeApi.DTO.UserDTO;



namespace ExchangeApi.Services
{
    public class UserService : IUserService 
    {
        private readonly DbConnect dbconnect;
        private readonly ILogger<UserService> logger;
        private readonly IJwtTokenService jwtTokenService;

        public UserService(DbConnect dbconnect, ILogger<UserService> logger,IJwtTokenService jwtTokenService) 
        {
            this.dbconnect = dbconnect;
            this.logger = logger;
            this.jwtTokenService = jwtTokenService;
        }

        public async Task<UsersInfo?> MakeRegister(string name, string phonenumber, string password)
        {
            try
            {
                var exist = await dbconnect.Set<UsersInfo>().FindAsync(phonenumber);
                if (exist != null)
                {
                    logger.LogInformation("Existed phone number try to login");
                    return null;
                }

                var user = new UsersInfo
                {
                    Name = name,
                    PhoneNumber = phonenumber,
                    Password = password

                };
                var hashedPassword = PasswordHasher.HashPassword(password);
                user.Password = hashedPassword;
                dbconnect.UsersInfo.Add(user);
                await dbconnect.SaveChangesAsync();

                logger.LogInformation("New user registered");
                return user;
            }
            catch (Exception)
            {
                logger.LogError("Error retrieving while register");
                return null;
            }
        }

        public async Task<LoginDto?> ValidateUser(string phonenumber, string password)
        {
            var user = await dbconnect.Set<UsersInfo>().FirstOrDefaultAsync(u => u.PhoneNumber == phonenumber);
            if (user == null) throw new KeyNotFoundException("User not found");

            if (PasswordHasher.VerifyPassword(password, user.Password))
            {
                var name = user.Name;

                var token = jwtTokenService.CreateJWTToken(phonenumber,name);
               
               
                logger.LogInformation("Generated Token: {Token}", token);
                return new LoginDto
                {
                    Token = token,
                    User = user
                };
            }
            else
                throw new UnauthorizedAccessException("Wrong password");
        }
    }
}

