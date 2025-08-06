using ExchangeApi.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using static ExchangeApi.DTO.UserDTO;

namespace ExchangeApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<UsersInfo?> MakeRegister(string name, string phonenumber, string password);
        Task<LoginDto?> ValidateUser(string phoneNumber, string password);
    }
}
