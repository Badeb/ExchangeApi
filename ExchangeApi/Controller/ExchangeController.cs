using ExchangeApi.Data;
using ExchangeApi.Models.Entities;
using ExchangeApi.Services;
using ExchangeApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ExchangeApi.Controllers

{
    //localhost/xxxx/api/....

    [ApiController]
    [Route("api/")]
    [Authorize]
   
    public class ExchangeController : ControllerBase
    {

        private readonly IExchangeService exchangeservice;
        private readonly ILogger<ExchangeController> logger;
        private readonly IJwtTokenService jwtTokenService;


        public ExchangeController(IExchangeService exchangeservice, ILogger<ExchangeController> logger , IJwtTokenService jwtTokenService)
        {

            this.exchangeservice = exchangeservice;
            this.logger = logger;
            this.jwtTokenService = jwtTokenService;

        }
        
        

        [HttpGet("exchange-rate")]
        public async Task<IActionResult> GetExchangeRate(string BaseCurrency, string TargetCurrency)
        {
            BaseCurrency = BaseCurrency.ToUpper();
            TargetCurrency = TargetCurrency.ToUpper();

      
            if (!ModelState.IsValid)
            {
                logger.LogError("Currency length is invalid for BaseCurrency:" + BaseCurrency + ", TargetCurrency:" + TargetCurrency);
               // return BadRequest(ModelState);
            }
            try
            {
                var result = await exchangeservice.SaveExchanges(BaseCurrency, TargetCurrency);
                if (result == null)
                {
                    logger.LogError("Exchange rates not found");
                    return NotFound("Exchange rates not found");
                }
                else
                {
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {

                logger.LogError(ex, "Error getting currencies  for {BaseCurrency} to {TargetCurrency} - {Message}", BaseCurrency, TargetCurrency, ex.Message);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpPost("addfav-queries")]
        public async Task<IActionResult> FavUsed(string BaseCurrency, string TargetCurrency)
        {
            BaseCurrency = BaseCurrency.ToUpper();
            TargetCurrency = TargetCurrency.ToUpper();
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userInfo = jwtTokenService.GetUserInfoFromToken(token);
            var name = userInfo.name.ToUpper();

            if (userInfo == null)
            {
                return NotFound("Name does not take ");
            }


            if (!ModelState.IsValid)
            {
                logger.LogError("Currency length is invalid for BaseCurrency:" + BaseCurrency + ", TargetCurrency:" + TargetCurrency);
                return BadRequest(ModelState);
            }
            try
            {
               
                var result = await exchangeservice.AddFavoriteQueries(BaseCurrency, TargetCurrency, name);
                if (result == null)
                {
                    logger.LogError("Exchange rates not found");
                    return NotFound("Exchange rates not found");
                }
                else
                {
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {

                logger.LogError(ex, "Error getting currencies  for {BaseCurrency} to {TargetCurrency} with {Name} - {Message}", BaseCurrency, TargetCurrency, name, ex.Message);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("fav-query")]
        public async Task<IActionResult> QueriesGet() //ListofFavQueries  by name
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userInfo = jwtTokenService.GetUserInfoFromToken(token);
            var name = userInfo.name.ToUpper();

            try
            {
               
                var list = await exchangeservice.ListOfFavQueries(name);
                if (list == null || list.Count == 0)
                {
                    logger.LogWarning("No fav queries found for that name");
                    return NotFound("No favorite queries found");
                }
                return Ok(list);
            }

            catch (Exception ex)
            {
                logger.LogError(ex, message: "Error getting queries  for  {Name} - {Message}", name, ex.Message);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpGet("currency-pair-results")]
        public async Task<IActionResult> ResultsGet(int Id)
        {
            var resultlist = await exchangeservice.ResultsOrderByTime(Id);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(resultlist);



        }

        [HttpDelete("remove_favorite")]
        public async Task<IActionResult> RemoveFav(int Id)  //if you want to delete your favorite base-target , you need to enter the id only
        {
            try
            {
                var deleteid = await exchangeservice.RemoveFromFavList(Id);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                return Ok("Favorite is delete");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error removing favorite with Id {Id}", Id);
                return StatusCode(500, "Internal server error");
            }

        }
    }
}