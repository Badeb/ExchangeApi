using ExchangeApi.Data;
using ExchangeApi.Models.Entities;
using ExchangeApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ExchangeApi.Controllers

{
    //localhost/xxxx/api/....

    [ApiController]
    [Route("api/")]
    public class ExchangeController : ControllerBase
    {
        private readonly DbConnect dbconnect;
        private readonly IExchangeService exchangeservice;
        private readonly ILogger<ExchangeController> logger;


        public ExchangeController(DbConnect dbconnect, IExchangeService exchangeservice, ILogger<ExchangeController> logger)
        {
            this.dbconnect = dbconnect;
            this.exchangeservice = exchangeservice;
            this.logger = logger;
        }


        [HttpGet("exchange-rate")]
        public async Task<IActionResult> GetExchangeRate(string BaseCurrency, string TargetCurrency)
        {
            BaseCurrency = BaseCurrency.ToUpper();
            TargetCurrency = TargetCurrency.ToUpper();

            //if (BaseCurrency == null || TargetCurrency == null)
            //{
            //    logger.LogInformation("Empty currency area in exchange-rate method");
            //    return BadRequest("Currency fields can not be empty");
            //}
            if (!ModelState.IsValid)
            {
                logger.LogError("Currency length is invalid for BaseCurrency:" + BaseCurrency + ", TargetCurrency:" + TargetCurrency);
                return BadRequest(ModelState);
            }
            try
            {
                var result = await exchangeservice.SaveExchanges(BaseCurrency, TargetCurrency);
                if (result == null)
                {
                    logger.LogError("Exchange rates not found");
                    return NotFound();
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



        [HttpPost("queries")]
        public async Task<IActionResult> FavUsed(string BaseCurrency, string TargetCurrency, string Name)
        {
            BaseCurrency = BaseCurrency.ToUpper();
            TargetCurrency = TargetCurrency.ToUpper();
            Name = Name.ToUpper();

            if (BaseCurrency == null || TargetCurrency == null)
            {
                logger.LogInformation("Empty currency area in exchange-rate method");
                return BadRequest("Currency fields can not be empty");
            }
            if (!ModelState.IsValid)
            {
                logger.LogError("Currency length is invalid for BaseCurrency:" + BaseCurrency + ", TargetCurrency:" + TargetCurrency);
                return BadRequest(ModelState);
            }
            try
            {
                var result = await exchangeservice.AddFavoriteQueries(BaseCurrency, TargetCurrency, Name);
                if (result == null)
                {
                    logger.LogError("Exchange rates not found");
                    return NotFound();
                }
                else
                {
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {

                logger.LogError(ex, "Error getting currencies  for {BaseCurrency} to {TargetCurrency} with {Name} - {Message}", BaseCurrency, TargetCurrency, ex.Message);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("queries")]
        public async Task<IActionResult> QueriesGet(string name) //ListofFavQueries  by name
        {
            try
            {
                name = name.ToUpper();
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

                logger.LogError(ex, message: "Error getting queries  for  {Name} - {Message}", name);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
           


        [HttpGet("results")]
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