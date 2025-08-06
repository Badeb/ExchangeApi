using ExchangeApi.Controllers;
using ExchangeApi.Data;
using ExchangeApi.Services;
using ExchangeApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using static ExchangeApi.DTO.UserDTO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ExchangeApi.Controllers
{
    [ApiController]
    [Route("api/")]


    public class UserController : ControllerBase
    {
        private readonly IUserService userservice;
        private readonly ILogger<UserController> logger;
        private readonly IJwtTokenService jwtTokenService;


        public UserController(IUserService userservice, IJwtTokenService jwtTokenService, ILogger<UserController> logger)
        { 
            this.userservice = userservice;
            this.jwtTokenService = jwtTokenService;
            this.logger = logger;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(string name,string phonenumber , string password)
        {
            //dto.Name = dto.Name.ToUpper();
            name = name.ToUpper();

            if (!ModelState.IsValid)
            {
                logger.LogError("Invalid entrance");
                return BadRequest(ModelState);
            }
            try
            {
                var result = await userservice.MakeRegister(name, phonenumber, password);
                if (result == null)
                {
                    logger.LogError("Phone number has already registered");
                    return BadRequest("Phone number has already registered.");
                }

                logger.LogInformation("New user registered");
                return Ok(result);

            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to register user: {phonenumber}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string phonenumber, string password) 
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Invalid entrance");
                return BadRequest(ModelState);
            }
            try
            {
                var result = await userservice.ValidateUser(phonenumber, password);
                if(result == null)
                {
                    logger.LogError("Checking error");
                    return BadRequest();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to login user: {phonenumber}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
        
}














